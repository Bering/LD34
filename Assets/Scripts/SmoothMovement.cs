using UnityEngine;
using System.Collections;

public class SmoothMovement : MonoBehaviour
{
	[SerializeField] protected float maxMovementSpeed;
	[SerializeField] protected float maxRotationSpeed;
	[SerializeField] protected float snapDistance; // When this close to target, we snap to it and be done with it

	protected Transform target; // Serializing this GREATLY helps for debugging
	protected float howQuickly; // 0 is pretty slow, 1 means use maxMovementSpeed

	protected Vector3 currentMovementSpeed;
	protected float distanceToTarget;
	protected float headingsDotProduct;


	public bool isAtDestination; /* { get; protected set; } */ // leaving fully public also GREATLY helps for debugging (set target, unchek isAtDestination, see movement)


	void Awake()
	{
		isAtDestination = true;
		howQuickly = 1.0f;
	}


	public void GoTo(Transform target, float howQuickly = 1)
	{
		this.howQuickly = howQuickly;
		this.target = target;
		isAtDestination = false;
	}


	void Update ()
	{
		if (isAtDestination)
			return;

		transform.position = Vector3.SmoothDamp(transform.position, target.position, ref currentMovementSpeed, howQuickly, maxMovementSpeed);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, target.rotation, maxRotationSpeed);

		distanceToTarget = Vector3.Distance (transform.position, target.position);
		headingsDotProduct = Quaternion.Dot (transform.rotation, target.rotation);

		if ((distanceToTarget < snapDistance) && (headingsDotProduct == 1)) {  // -1 = parallel but opposite, 1 = parallel and same direction
			transform.position = target.position;
			transform.rotation = target.rotation;
			isAtDestination = true;
			distanceToTarget = 0;
		}
	}
}

