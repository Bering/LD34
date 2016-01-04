using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour {

	static public InputHandler instance;

	public UnityEvent LeftButtonEvent;
	public UnityEvent RightButtonEvent;

	protected GameController gc;

	protected bool acceptInput;


	static public InputHandler GetInstance()
	{
		if (instance != null) return instance;

		throw new System.Exception("Something tried to get the instance before it was instanciated. Check the script load order.");
	}


	void Awake()
	{
		instance = this;
		acceptInput = false;
	}


	void Start()
	{
		gc = GameController.GetInstance();
		gc.EnzymesReadyEvent.AddListener (AcceptInput);
		gc.GamePausedEvent.AddListener (RejectInput);
		gc.GameResumedEvent.AddListener (RejectInput);
	}


	void AcceptInput()
	{
		acceptInput = true;
	}


	void RejectInput()
	{
		acceptInput = false;
	}


	void Update()
	{
		#if UNITY_ANDROID
		return;
		#endif

		if (Input.GetKeyDown(KeyCode.Escape)) {
			gc.PauseGame ();
			return;
		}

		if (!acceptInput)
			return;
		
		if (Input.GetAxis("Horizontal") < -0.2f || Input.GetButtonDown("Fire1")) {
			acceptInput = false;
			LeftButtonEvent.Invoke();
		}
		if (Input.GetAxis("Horizontal") > 0.2f || Input.GetButtonDown("Fire2")) {
			acceptInput = false;
			RightButtonEvent.Invoke();
		}

	}

}
