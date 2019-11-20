using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Vehicle
{
	// A list of all the zombies in the scene
	public List<GameObject> zombies;

	// The PSG that the human will seek
	public GameObject PSG;

	public override void Update()
	{
		// Gets an updated list of the humans in the scene 
		zombies = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().zombies;

		base.Update();
	}

	public override void CalcSterringForces()
	{
		Vector3 ultamiteForce = Vector3.zero;

		foreach(GameObject zombie in zombies)
		{
			// When a zombie is less than 3 units away, Flee() is called
			if(Vector3.Distance(transform.position, zombie.transform.position) < 4f)    
			{
				ultamiteForce += Flee(zombie);
			}
			// When a zombie is less than 5 units away, Evade() is called
			else if(Vector3.Distance(transform.position, zombie.transform.position) < 5f)
			{
				ultamiteForce += Evade(zombie);
			}
			else	// Otherwise, it will wander
			{
				ApplyForce(Wander());
			}
		}

		//ultamiteForce += Seek(PSG);

		ultamiteForce = ultamiteForce * maxSpeed;

		ApplyForce(ultamiteForce);
	}
}
