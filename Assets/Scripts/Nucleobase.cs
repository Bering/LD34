using UnityEngine;
using System.Collections;

[System.Serializable]
public class Nucleobase
{
	
	public enum types
	{
		A,
		C,
		G,
		T
	}

	public types type;
	public types pairsWith;

}

