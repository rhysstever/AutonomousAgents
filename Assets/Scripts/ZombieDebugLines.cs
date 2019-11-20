using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDebugLines : MonoBehaviour
{
	// Materials for debugging, will be 
	// assigned when the gameObject is created
	public Material debugGreen; // Green -	forward vector
	public Material debugBlue;  // Blue  -	right vector
	public Material debugBlack; // Black -	seeking line to closest human
	public Material debugRed;   // Red	 -	future position

	void OnRenderObject()
	{
		if(GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().isDebugging)
		{
			if(gameObject.GetComponent<Zombie>().closestHuman != null)
			{
				// Draws a black line to the human that it is seeking
				debugBlack.SetPass(0);
				GL.Begin(GL.LINES);
				GL.Vertex(transform.position);
				GL.Vertex(gameObject.GetComponent<Zombie>().closestHuman.transform.position);
				GL.End();
			}

			// Draws a green debug line for the forward vector
			debugGreen.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex(transform.position);
			GL.Vertex(transform.position + gameObject.GetComponent<Vehicle>().direction * 3f);
			GL.End();

			// Draws a blue debug line for the right vector
			debugBlue.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex(transform.position);
			GL.Vertex(transform.position +
				Vector3.Cross(Vector3.up, gameObject.GetComponent<Vehicle>().direction) * 2f);
			GL.End();

			// Draws a red debug dot on the zombie's future position
			debugRed.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex(transform.position);
			GL.Vertex(transform.position + gameObject.GetComponent<Vehicle>().velocity * 3);
			GL.End();
		}
	}
}
