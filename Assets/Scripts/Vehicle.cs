using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour
{
	// Vectors necessary for force-based movement
	private Vector3 vehiclePosition;
	public Vector3 acceleration;
	public Vector3 direction;
	public Vector3 velocity;

	// Floats
	public float mass;
	public float maxSpeed;
	public float radius;

	public GameObject currentObstacle;
	public bool isAvoiding;
	List<GameObject> obstacles;

	// Start is called before the first frame update
	public void Start()
    {
		vehiclePosition = transform.position;
		radius = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.x / 2;
	}

    // Update is called once per frame
    public virtual void Update()
    {
		CalcSterringForces();
		StayInBounds(40f);

		obstacles = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().obstacles;
		foreach(GameObject obstacle in obstacles)
		{
			currentObstacle = obstacle;
			ApplyForce(ObstacleAvoidance(currentObstacle, 5f) * 5);
		}

		velocity += acceleration * Time.deltaTime;
		velocity.y = 0f;
		vehiclePosition += velocity * Time.deltaTime;
		direction = velocity.normalized;
		acceleration = Vector3.zero;
		transform.position = vehiclePosition;
		transform.rotation = Quaternion.LookRotation(direction);
	}

	public void ApplyForce(Vector3 force)
	{
		acceleration += force / mass;
	}

	public void ApplyFriction(float coeff)
	{
		Vector3 friction = velocity * -1;
		friction.Normalize();
		friction = friction * coeff;
		acceleration += friction;
	}

	/// <summary>
	/// Seek
	/// </summary>
	/// <param name="targetPosition">Position of desired target</param>
	/// <returns>Steering force to get to target position</returns>
	public Vector3 Seek(Vector3 targetPosition)
	{
		// Step 1: Find DV (desired velocity)
		// TargetPos - CurrentPos
		Vector3 desiredVelocity = targetPosition - vehiclePosition;

		// Step 2: Scale vel to max speed
		// desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
		desiredVelocity.Normalize();
		desiredVelocity = desiredVelocity * maxSpeed;

		// Step 3:  Calculate seeking steering force
		Vector3 seekingForce = desiredVelocity - velocity;

		// Step 4: Return force
		return seekingForce;
	}

	/// <summary>
	/// Overloaded Seek
	/// </summary>
	/// <param name="target">GameObject of desired target</param>
	/// <returns>Steering force to get to target position</returns>
	public Vector3 Seek(GameObject target)
	{
		return Seek(target.transform.position);
	}

	/// <summary>
	/// Flee
	/// </summary>
	/// <param name="targetPosition">Position of undesired target</param>
	/// <returns>Steering force to get away from target position</returns>
	public Vector3 Flee(Vector3 targetPosition)
	{
		Vector3 desiredVel = targetPosition - vehiclePosition;

		Vector3 desiredFleeVel = -desiredVel;
		desiredFleeVel.Normalize();
		desiredFleeVel = desiredFleeVel * maxSpeed;

		Vector3 fleeingForce = desiredFleeVel - velocity;

		return fleeingForce;
	}

	/// <summary>
	/// Overloaded Flee
	/// </summary>
	/// <param name="target">GameObject of undesired target</param>
	/// <returns>Steering force to get away from target position</returns>
	public Vector3 Flee(GameObject target)
	{
		return Flee(target.transform.position);
	}

	/// <summary>
	/// Calculates a steering force to seek a target by anticipating where 
	/// it will be in 3 frames
	/// </summary>
	/// <param name="target">The gameObject this Vehicle is seeking</param>
	/// <returns>A Vector3 for going to where the target will be in 3 frames</returns>
	public Vector3 Pursue(GameObject target)
	{
		return Seek(target.transform.position + (target.GetComponent<Vehicle>().velocity * 2f));
	}

	/// <summary>
	/// Calculates a steering force to flee a target by anticipating where 
	/// it will be in 3 frames
	/// </summary>
	/// <param name="target">The gameObject this Vehicle is fleeing from</param>
	/// <returns>A Vector3 for running away from where the target will be in 3 frames</returns>
	public Vector3 Evade(GameObject target)
	{
		return Flee(target.transform.position + (target.GetComponent<Vehicle>().velocity * 2f));
	}

	public Vector3 Wander()
	{
		return Wander(2f);
	}

	public Vector3 Wander(float radius)
	{
		float randAngle = Random.Range(0, 2 * Mathf.PI);
		Vector3 randVector = new Vector3(Mathf.Cos(randAngle), 0, Mathf.Sin(randAngle));

		return Seek(vehiclePosition + (direction * 3) + randVector * radius);
	}

	public abstract void CalcSterringForces();

	/// <summary>
	/// Makes the vehicle seek the center of the scene whenever is moves outside the bounds
	/// </summary>
	/// <param name="bound">The distance that marks the edge of the acceptable area</param>
	void StayInBounds(float bound)
	{
		if(Mathf.Abs(vehiclePosition.x) > bound)
			ApplyForce(Seek(Vector3.zero));

		if(Mathf.Abs(vehiclePosition.z) > bound)
			ApplyForce(Seek(Vector3.zero) * 10);
	}

	/// <summary>
	/// Avoids a specified obstacle by returning a steering force that will be applied to 
	/// the vehicle to move around the obstacle
	/// </summary>
	/// <param name="obstacle">The obstacle the vehicle is avoiding</param>
	/// <param name="safeDist">The specified distance away from an obstacle that is deemed "safe"</param>
	/// <returns>Returns a steering force that will be applied to the object to avoid the obstacle</returns>
	Vector3 ObstacleAvoidance(GameObject obstacle, float safeDist)
	{
		isAvoiding = false;
		Vector3 obsPos = obstacle.transform.position;
		
		// If the target is not front of me, the dot product of the 
		// vehicle and the obstacle will be less than 0
		if(Vector3.Dot(direction, obsPos - vehiclePosition) < 0)
			return Vector3.zero;

		// If the target is not within a specified range
		if(Vector3.Distance(transform.position, obsPos) > safeDist)
			return Vector3.zero;

		Vector3 right = Vector3.Cross(Vector3.up, direction);
		isAvoiding = true;

		// Non-intersection test
		if(Mathf.Abs(Vector3.Dot(right, obsPos - vehiclePosition)) > 
			(obstacle.GetComponent<Obstacle>().radius + radius))
			return Vector3.zero;

		// Is it to the right or left
		if(Vector3.Dot(right, obsPos - vehiclePosition) >= 0)
			return ObstacleAvoidanceSeek(-right);   // If right, dodge left
		else
			return ObstacleAvoidanceSeek(right);    // If left, dodge right
	}

	/// <summary>
	/// A variant Seek method that is used for Object Avoidance
	/// </summary>
	/// <param name="desiredVelocity">The velocity that the object wishes to be at</param>
	/// <returns>Returns a steering force that will be applied to the object to avoid the obstacle</returns>
	Vector3 ObstacleAvoidanceSeek(Vector3 desiredVelocity)
	{
		desiredVelocity.Normalize();
		desiredVelocity = desiredVelocity * maxSpeed;

		Vector3 seekingForce = desiredVelocity - velocity;

		return seekingForce;
	}
}