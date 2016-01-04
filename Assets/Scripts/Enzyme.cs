using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Enzyme : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] protected PluckablesList Pluckables;
	[SerializeField] protected Transform FusingMarker;
	[SerializeField] protected bool isLeft;

	protected InputHandler inputHandler;
	protected GameController gameController;
	protected SmoothMovement movementController;
	protected AudioSource audioSource;

	public bool isReadyForTurnStart;

	protected Nucleobase_View nucleobaseToPluck;


	void Start()
	{
		inputHandler = InputHandler.GetInstance ();
		gameController = GameController.GetInstance ();
		movementController = GetComponent<SmoothMovement> ();
		audioSource = GetComponent<AudioSource>();

		if (isLeft) {
			inputHandler.LeftButtonEvent.AddListener (OnButtonPressed);
		} else {
			inputHandler.RightButtonEvent.AddListener (OnButtonPressed);
		}
	}


	public void SelectNucleobase(Nucleobase_View nucleobaseToPluck)
	{
		this.nucleobaseToPluck = nucleobaseToPluck;
		isReadyForTurnStart = false;
		movementController.GoTo(nucleobaseToPluck.transform, gameController.cycleMovementDuration);
		StartCoroutine(SetReadyWhenDestinationReached ());
	}


	protected IEnumerator SetReadyWhenDestinationReached()
	{
		yield return WaitForCompleteStop ();

		isReadyForTurnStart = true;
	}


	protected IEnumerator WaitForCompleteStop()
	{
		while (!movementController.isAtDestination) {
			yield return new WaitForSeconds (0.25f);
		}
	}


	public void OnButtonPressed()
	{
		StartCoroutine (Fuse());
	}


	IEnumerator Fuse()
	{
		isReadyForTurnStart = false;

		// Pluck
		nucleobaseToPluck.transform.parent = transform;
		audioSource.clip = gameController.pluckSound;
		audioSource.Play ();

		// Bring to fusion marker
		movementController.GoTo(FusingMarker.transform, gameController.cycleMovementDuration);
		yield return StartCoroutine (WaitForCompleteStop ());

		// Fuse
		nucleobaseToPluck.transform.parent = FusingMarker.parent;
		audioSource.clip = gameController.fuseSound;
		audioSource.Play ();

		// Little delay for better look
		yield return new WaitForSeconds (0.5f);

		gameController.NextTurn ();
	}


	public void OnPointerClick(PointerEventData eventData)
	{
		#if UNITY_ANDROID
		if (isLeft) {
			inputHandler.LeftButtonEvent.Invoke();
		} else{
			inputHandler.RightButtonEvent.Invoke();
		}
		#endif

		eventData.Use();
	}

}

