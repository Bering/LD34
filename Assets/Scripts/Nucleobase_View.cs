using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Nucleobase_View : MonoBehaviour
{
	static public List<Nucleobase_View> List = new List<Nucleobase_View>();

	[SerializeField] protected Nucleobase nucleobaseModel;
	[SerializeField] protected Text uiLabel;

	protected Vector3 cyclePositionIncrement;
	protected Vector3 targetPosition;
	protected Vector3 currentSpeed;

	protected /*public*/ bool isAtDestination;


	void Awake()
	{
		List.Add (this);
	}


	void OnDestroy()
	{
		List.Remove (this);
	}


	void Start()
	{
		uiLabel.text = this.nucleobaseModel.type.ToString ();

		// TODO: Fix this!
		uiLabel.rectTransform.Rotate(0, 0, transform.rotation.eulerAngles.z * -1.0f); // Rotate the text so it's upright

		GameController.GetInstance ().NextTurnEvent.AddListener(OnNextCycle);
		targetPosition = transform.position;
		isAtDestination = true;
	}


	static public bool allAtDestination()
	{
		foreach(Nucleobase_View nb in Nucleobase_View.List) {
			if (!nb.isAtDestination) {
				return false;
			}
		}

		return true;
	}


	static public Nucleobase_View GetNucleobaseAtPosition(Vector3 position)
	{
		foreach(Nucleobase_View nb in Nucleobase_View.List) {
			if (Vector3.Distance(nb.transform.position, position) < 0.5f) {
				return nb;
			}
		}

		return null;
	}


	/**
	 * Above are static
	 * */


	public void Setup(Vector3 pos, Quaternion rot, Vector3 cycleMovementDirection)
	{
		transform.position = pos;
		transform.rotation = rot;

		cyclePositionIncrement = cycleMovementDirection * 10;
	}


	public Nucleobase.types getNucleobaseType()
	{
		return nucleobaseModel.type;
	}


	public Nucleobase.types getNucleobaseComplementType()
	{
		return nucleobaseModel.pairsWith;
	}


	void OnNextCycle()
	{
		targetPosition = transform.position + cyclePositionIncrement;
		isAtDestination = false;
	}


	void Update()
	{
		if (isAtDestination)
			return;
	
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentSpeed, GameController.GetInstance().cycleMovementDuration);

		if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
			transform.position = targetPosition;
			isAtDestination = true;
		}
	}

}

