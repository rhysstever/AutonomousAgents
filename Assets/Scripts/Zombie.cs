using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Vehicle
{
	// A list of all humans in the scene
	public List<GameObject> humans;

	// The closest human (and its distance)
	// that the zombie will seek
	public GameObject closestHuman;
	float distToHuman;

	public override void Update()
	{
		// Gets an updated list of the humans in the scene 
		humans = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().humans;

		base.Update();
	}

	public override void CalcSterringForces()
	{
		distToHuman = float.MaxValue;
		Vector3 ultamiteForce = Vector3.zero;
		closestHuman = null;

		foreach(GameObject human in humans)
		{
			if(Vector3.Distance(human.transform.position, transform.position) < distToHuman)
			{
				distToHuman = Vector3.Distance(human.transform.position, transform.position);
				closestHuman = human;
			}
		}

		if(closestHuman != null)
		{
			if(Vector3.Distance(transform.position, closestHuman.transform.position) > 3f)
			{
				ultamiteForce += Pursue(closestHuman);
			}
			else
			{
				ultamiteForce += Seek(closestHuman);
			}
		}
		else
			ultamiteForce = Wander();

		ultamiteForce = ultamiteForce * maxSpeed;

		ApplyForce(new Vector3(ultamiteForce.x, 0.0f, ultamiteForce.z));
	}
}
