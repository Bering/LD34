using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SnapPositionToInt : MonoBehaviour
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
		
		pos = transform.position;
		pos.x = Mathf.RoundToInt(pos.x);
		pos.y = Mathf.RoundToInt(pos.y);
		pos.z = Mathf.RoundToInt(pos.z);
		transform.position = pos;
	}

}

