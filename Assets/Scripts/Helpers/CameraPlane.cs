using UnityEngine;
using System.Collections;

public class CameraPlane : MonoBehaviour {

	Camera c;
	Vector3 topLeft;
	Vector3 topRight;
	Vector3 bottomRight;
	Vector3 bottomLeft;


	void OnDrawGizmos()
	{
		if (c == null) {
			c = GetComponent<Camera> ();
		}

		//Gizmos.DrawFrustum(c.transform.position, c.fieldOfView, c.farClipPlane, c.nearClipPlane, c.aspect);

		topLeft = c.ViewportToWorldPoint (new Vector3 (0, 1, -c.transform.position.z));
		topRight = c.ViewportToWorldPoint (new Vector3 (1, 1, -c.transform.position.z));;
		bottomRight = c.ViewportToWorldPoint (new Vector3 (1, 0, -c.transform.position.z));;
		bottomLeft = c.ViewportToWorldPoint (new Vector3 (0, 0, -c.transform.position.z));;

		Gizmos.DrawLine (topLeft, topRight);
		Gizmos.DrawLine (topRight, bottomRight);
		Gizmos.DrawLine (bottomRight, bottomLeft);
		Gizmos.DrawLine (bottomLeft, topLeft);
	}

}
