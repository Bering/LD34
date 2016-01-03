using UnityEngine;
using System.Collections.Generic;

public class PluckablesList : MonoBehaviour {


	public List<Nucleobase_View> list;


	void Awake() {
		list = new List<Nucleobase_View>();
	}


	void OnTriggerEnter(Collider other)
	{
		list.Add(other.gameObject.GetComponent<Nucleobase_View>());
	}


	void OnTriggerExit(Collider other)
	{
		list.Remove (other.gameObject.GetComponent<Nucleobase_View>());
	}


	public Nucleobase_View GetNucleobaseOfType(Nucleobase.types type)
	{
		foreach (Nucleobase_View nv in list) {
			if (nv.getNucleobaseType () == type)
				return nv;
		}

		return null;
	}

	public Nucleobase_View GetRandomNucleobase()
	{
		if (list.Count == 0) {
			return null;
		}

		int index = Random.Range (0, list.Count);

		foreach (Nucleobase_View nv in list) {
			if (index == 0) {
				return nv;
			}
			index--;
		}

		return null;
	}
	

}
