using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class GameController : MonoBehaviour {

	static protected GameController instance;

	[SerializeField] protected GameObject AdeninePrefab;
	[SerializeField] protected GameObject CytosinePrefab;
	[SerializeField] protected GameObject GuaninePrefab;
	[SerializeField] protected GameObject ThyminePrefab;

	[SerializeField] protected Sequence[] Sequences;

	[SerializeField] protected Enzyme LeftEnzyme;
	[SerializeField] protected Enzyme RightEnzyme;
	[SerializeField] protected PluckablesList Pluckables;

	[SerializeField] protected Transform NucleobaseToComplementMarker;

	public float cycleMovementDuration;
	public int scoreBonus;
	public int scoreMalus;

	public UnityEvent GamePausedEvent;
	public UnityEvent GameResumedEvent;
	public UnityEvent GameOverEvent;

	public UnityEvent FirstTurnStartedEvent;
	public UnityEvent NucleobasesReadyEvent;
	public UnityEvent EnzymesReadyEvent;
	public UnityEvent NextTurnEvent;

	public AudioSource audioSource;
	public AudioClip scoreBonusSound;
	public AudioClip scoreMalusSound;
	public AudioClip pluckSound;
	public AudioClip fuseSound;


	protected bool leftHasGoodAnswer;
	protected bool rightHasGoodAnswer;


	static public GameController GetInstance()
	{
		if (instance != null) return instance;

		throw new System.Exception("Something tried to get the GameController instance before it was instanciated. Check the script load order.");
	}


	void Awake()
	{
		instance = this;
	}


	void Start()
	{
		InputHandler.GetInstance().LeftButtonEvent.AddListener (GetInstance().OnLeftButtonPressed);
		InputHandler.GetInstance().RightButtonEvent.AddListener (GetInstance().OnRightButtonPressed);
		NucleobasesReadyEvent.AddListener (EnzymesAI);
	}


	public void QuitGame()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}


	public void PauseGame()
	{
		GamePausedEvent.Invoke();
		Time.timeScale = 0;
	}


	public void ResumeGame()
	{
		GameResumedEvent.Invoke();
		Time.timeScale = 1;
	}


	public void StartGame()
	{
		FirstTurnStartedEvent.Invoke ();
		StartCoroutine (CycleTurn ());
	}


	public void NextTurn()
	{
		NextTurnEvent.Invoke ();
		StartCoroutine (CycleTurn ());
	}


	public void GameOver()
	{
		GameOverEvent.Invoke ();
		Time.timeScale = 0;
	}

	protected IEnumerator CycleTurn()
	{
		Debug.Log ("Preparing Turn");

		foreach (Sequence s in Sequences) {
			if (!s.isReadyForTurnStart)
				yield return new WaitForSeconds (0.25f);
		}

		while (!Nucleobase_View.allAtDestination())
			yield return new WaitForSeconds (0.25f);

		Debug.Log ("Nucleobases are ready");
		NucleobasesReadyEvent.Invoke ();

		while (!LeftEnzyme.isReadyForTurnStart)
			yield return new WaitForSeconds (0.25f);

		while (!RightEnzyme.isReadyForTurnStart)
			yield return new WaitForSeconds (0.25f);

		Debug.Log ("Enzymes are ready");
		EnzymesReadyEvent.Invoke ();
	}


	void OnLeftButtonPressed()
	{
		Score (leftHasGoodAnswer);
	}


	void OnRightButtonPressed()
	{
		Score (rightHasGoodAnswer);
	}


	public Nucleobase_View SpawnNucleobaseFromType(Nucleobase.types type)
	{
		switch (type) {
		case Nucleobase.types.A: return Instantiate(AdeninePrefab).GetComponent<Nucleobase_View>();
		case Nucleobase.types.C: return Instantiate(CytosinePrefab).GetComponent<Nucleobase_View>();
		case Nucleobase.types.G: return Instantiate(GuaninePrefab).GetComponent<Nucleobase_View>();
		case Nucleobase.types.T: return Instantiate(ThyminePrefab).GetComponent<Nucleobase_View>();
		default:
			throw new System.Exception ("Invalid Nucleobase type!");
		}
	}
	
	
	public bool isReadyForTurnStart()
	{
		foreach (Sequence s in Sequences) {
			if (!s.isReadyForTurnStart)
				return false;
		}

		if (!Nucleobase_View.allAtDestination())
			return false;

		if (!LeftEnzyme.isReadyForTurnStart)
			return false;

		if (!RightEnzyme.isReadyForTurnStart)
			return false;

		return true;
	}
	

	public Nucleobase_View GetNucleobaseToComplement()
	{
		return Nucleobase_View.GetNucleobaseAtPosition (NucleobaseToComplementMarker.position);
	}


	[ContextMenu("Select Nucleobases")]
	public void EnzymesAI()
	{
		Nucleobase_View NucleobaseToComplement = Nucleobase_View.GetNucleobaseAtPosition (NucleobaseToComplementMarker.position);
		Nucleobase.types goodType = NucleobaseToComplement.getNucleobaseComplementType();

		Nucleobase_View NucleobaseToPluck_1 = null;
		Nucleobase_View NucleobaseToPluck_2 = null;

		Pluckables.ResetAvailables();

		if (Random.Range (0, 2) == 0) {
			NucleobaseToPluck_1 = Pluckables.GetNucleobaseOfType (goodType);
			NucleobaseToPluck_2 = Pluckables.GetRandomNucleobase ();
		} else {
			NucleobaseToPluck_1 = Pluckables.GetRandomNucleobase ();
			NucleobaseToPluck_2 = Pluckables.GetNucleobaseOfType (goodType);
		}


		if (NucleobaseToPluck_1 == null) {
			Debug.Log ("First pluckable is null! Trying a random one.");
			NucleobaseToPluck_1 = Pluckables.GetRandomNucleobase();
		}

		if (NucleobaseToPluck_2 == null) {
			Debug.Log ("Second pluckable is null! Trying a random one.");
			NucleobaseToPluck_2 = Pluckables.GetRandomNucleobase();
		}
			
		if (NucleobaseToPluck_1 == null || NucleobaseToPluck_2 == null) {
			Debug.LogError ("Out of Nucleobases to pluck !?!?");
			return;
		}



		if (NucleobaseToPluck_1.transform.position.x < NucleobaseToPluck_2.transform.position.x) {
			LeftEnzyme.SelectNucleobase (NucleobaseToPluck_1);
			leftHasGoodAnswer = (NucleobaseToPluck_1.getNucleobaseType() == goodType);

			RightEnzyme.SelectNucleobase (NucleobaseToPluck_2);
			rightHasGoodAnswer = (NucleobaseToPluck_2.getNucleobaseType() == goodType);
		} else {
			LeftEnzyme.SelectNucleobase (NucleobaseToPluck_2);
			leftHasGoodAnswer = (NucleobaseToPluck_2.getNucleobaseType() == goodType);

			RightEnzyme.SelectNucleobase (NucleobaseToPluck_1);
			rightHasGoodAnswer = (NucleobaseToPluck_1.getNucleobaseType() == goodType);
		}

	}


	public void Score(bool goodOrBad) {

		if (goodOrBad) {
			//scoreBonusSound.Play ();
			ScoreBoard.instance.IncrementScore (scoreBonus);
		} else {
			//scoreMalusSound.Play ();
			if (ScoreBoard.instance.IncrementScore (scoreMalus) < 0) {
				GameOver();
			};
		}

	}

}
