using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class LockToZAxis : MonoBehaviour
{
	protected Vector3 pos;

	void Update()
	{
		if (Application.isPlaying) {
			Destroy (this);
		}

		#if UNITY_EDITOR
		if (Selection.activeGameObject != gameObject)
			return;
		#endif

		if (transform.position.z == 0)
			return;
		
		pos = transform.position;
		pos.z = 0;
		transform.position = pos;
	}
	
}

