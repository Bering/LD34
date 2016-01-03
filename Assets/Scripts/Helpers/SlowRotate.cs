using UnityEngine;

public class SlowRotate : MonoBehaviour {

	public Vector3 speed;

	void Update () {
		transform.Rotate (speed.z * Time.deltaTime, speed.x * Time.deltaTime, speed.y * Time.deltaTime);
	}

}
