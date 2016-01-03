using UnityEngine;
using System.Collections;

public class Enzyme : MonoBehaviour
{
	[SerializeField] protected PluckablesList Pluckables;
	[SerializeField] protected Transform FusingMarker;
	[SerializeField] protected bool isLeft;

	protected GameController gameController;
	protected SmoothMovement movementController;

	public bool isReadyForTurnStart;

	protected Nucleobase_View NucleobaseToComplement;
	protected Nucleobase_View NucleobaseToPluck;


	void Start()
	{
		gameController = GameController.GetInstance ();
		movementController = GetComponent<SmoothMovement> ();

		gameController.NucleobasesReadyEvent.AddListener (OnNucleobasesReady);
		if (isLeft) {
			InputHandler.GetInstance ().LeftButtonEvent.AddListener (OnButtonPressed);
		} else {
			InputHandler.GetInstance().RightButtonEvent.AddListener (OnButtonPressed);
		}
	}


	public void OnNucleobasesReady()
	{
		NucleobaseToComplement = gameController.GetNucleobaseToComplement ();
		NucleobaseToPluck = Pluckables.GetNucleobaseOfType (NucleobaseToComplement.getNucleobaseComplementType ());

		if (NucleobaseToPluck == null) {
			Debug.Log ("No appropriate Nucleobase found!");
			NucleobaseToPluck = Pluckables.GetRandomNucleobase();
		}

		if (NucleobaseToPluck == null) {
			Debug.LogError ("Out of Nucleobases!");
			gameController.GameOverEvent.Invoke ();
			return;
		}

		isReadyForTurnStart = false;
		movementController.GoTo(NucleobaseToPluck.transform, gameController.cycleMovementDuration);
		StartCoroutine (WaitForCompleteStop ());
	}


	protected IEnumerator WaitForCompleteStop()
	{
		while (!movementController.isAtDestination) {
			yield return new WaitForSeconds (0.25f);
		}

		isReadyForTurnStart = true;
	}


	public void OnButtonPressed()
	{
		StartCoroutine (Fuse ());
	}


	IEnumerator Fuse()
	{
		isReadyForTurnStart = false;

		gameController.audioSource.clip = gameController.pluckSound;
		gameController.audioSource.Play ();

		NucleobaseToPluck.gameObject.transform.parent = transform;
		movementController.GoTo(FusingMarker.transform, gameController.cycleMovementDuration);
		yield return StartCoroutine (WaitForCompleteStop ());

		gameController.audioSource.clip = gameController.fuseSound;
		gameController.audioSource.Play ();

		NucleobaseToPluck.Fuse (FusingMarker.parent);
		yield return StartCoroutine (WaitForCompleteStop ());


		if (NucleobaseToPluck.getNucleobaseType() == NucleobaseToComplement.getNucleobaseComplementType ()) {
			gameController.Score (true);
		} else {
			gameController.Score (false);
		}


		isReadyForTurnStart = true;

		gameController.NextTurn ();
	}


}

