using UnityEngine;
using System.Collections;

public class Sequence : MonoBehaviour
{

	[SerializeField] protected Transform SpawnMarker;
	[SerializeField] protected Transform ActionMarker;
	[SerializeField] bool complement;

	protected Vector3 movementDirection;

	public bool isReadyForTurnStart;


	void Awake()
	{
		movementDirection = Vector3.Normalize (ActionMarker.position - SpawnMarker.position);
		isReadyForTurnStart = false;
	}


	void Start()
	{
		GameController gc = GameController.GetInstance ();
		gc.FirstTurnStartedEvent.AddListener (Preload);
		gc.NextTurnEvent.AddListener (NextTurn);
	}


	public void SetupNewNucleobase(Nucleobase_View newRandomNucleobase)
	{
		newRandomNucleobase.Setup (SpawnMarker.position, SpawnMarker.rotation, movementDirection);
		newRandomNucleobase.transform.parent = transform;
	}


	public void Preload()
	{
		isReadyForTurnStart = false;

		GameController gc = GameController.GetInstance ();
		Nucleobase_View newRandomNucleobase = null;
		Vector3 pos = SpawnMarker.position;

		int failSafeCounter = 0; // to prevent infinite loops

		while(true) {
			newRandomNucleobase = gc.SpawnRandomNucleobase ();
			SetupNewNucleobase (newRandomNucleobase);
			newRandomNucleobase.transform.position = pos;

			if (complement) {
				newRandomNucleobase = gc.SpawnNucleobaseFromType (newRandomNucleobase.getNucleobaseComplementType ());
				SetupNewNucleobase(newRandomNucleobase);
				newRandomNucleobase.transform.position = pos;
				newRandomNucleobase.transform.Translate (0, 6, 0);
				newRandomNucleobase.transform.Rotate (180, 0, 0);
			}

			if (Vector3.Distance (pos, ActionMarker.position) <= 0.1f)
				break;

			if (++failSafeCounter >= 30) {
				Debug.LogError("FailSafeCounter reached when preloading sequence " + this.gameObject.name);
				break;
			}
			
			pos += movementDirection * 10;
		};

		isReadyForTurnStart = true;
	}


	public void NextTurn()
	{
		isReadyForTurnStart = false;

		Nucleobase_View newRandomNucleobase = GameController.GetInstance ().SpawnRandomNucleobase ();
		SetupNewNucleobase (newRandomNucleobase);

		if (complement) {
			newRandomNucleobase = GameController.GetInstance().SpawnNucleobaseFromType (newRandomNucleobase.getNucleobaseComplementType ());
			SetupNewNucleobase(newRandomNucleobase);
			newRandomNucleobase.transform.Translate (0, 6, 0);
			newRandomNucleobase.transform.Rotate (180, 0, 0);
		}

		isReadyForTurnStart = true;
	}


	void OnDrawGizmos()
	{
		Gizmos.DrawLine (SpawnMarker.position, ActionMarker.position);
		if (complement) {
			Gizmos.DrawLine (SpawnMarker.position + new Vector3 (0, -6, 0), ActionMarker.position + new Vector3 (0, -6, 0));
		}
	}

}

