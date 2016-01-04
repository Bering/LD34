using UnityEngine;
using System.Collections.Generic;

public class PluckablesList : MonoBehaviour {


	[SerializeField] protected List<Nucleobase_View> list; // Serialized for debugging
	[SerializeField] protected List<Nucleobase_View> availableForPlucking;


	void Awake() {
		list = new List<Nucleobase_View>();
		availableForPlucking= new List<Nucleobase_View>();
	}


	void OnTriggerEnter(Collider other)
	{
		list.Add(other.gameObject.GetComponent<Nucleobase_View>());
	}


	void OnTriggerExit(Collider other)
	{
		list.Remove (other.gameObject.GetComponent<Nucleobase_View>());
	}


	public void ResetAvailables()
	{
		availableForPlucking = new List<Nucleobase_View>(list);
	}


	public Nucleobase_View GetNucleobaseOfType(Nucleobase.types type)
	{
		foreach (Nucleobase_View nv in availableForPlucking) {
			if (nv.getNucleobaseType () == type) {
				availableForPlucking.Remove(nv);
				return nv;
			}
		}

		return null;
	}


	public Nucleobase_View GetRandomNucleobase()
	{
		int index = Random.Range (0, availableForPlucking.Count);

		foreach (Nucleobase_View nv in availableForPlucking) {
			if (index == 0) {
				availableForPlucking.Remove(nv);
				return nv;
			}
			index--;
		}

		return null;
	}

}
