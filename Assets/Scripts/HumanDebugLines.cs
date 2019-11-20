using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanDebugLines : MonoBehaviour
{
	// Materials for debugging, will be 
	// assigned when the gameObject is created
	public Material debugGreen;		// Green	-	forward vector
	public Material debugBlue;		// Blue		-	right vector
	public Material debugPurple;	// Purple	-	future position

	void OnRenderObject()
	{
		if(GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().isDebugging)
		{
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

			// Draws a purple dot for the future position of the human
			debugPurple.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex(transform.position);
			GL.Vertex(transform.position + gameObject.GetComponent<Vehicle>().velocity * 3);
			GL.End();
		}
	}
}
