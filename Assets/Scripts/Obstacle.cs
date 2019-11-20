using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	public float radius;
	
	// Start is called before the first frame update
    void Start()
    {
		radius = gameObject.GetComponent<MeshRenderer>().bounds.size.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
