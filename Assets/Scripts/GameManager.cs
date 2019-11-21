using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	// Human/zombie prefabs
	public GameObject humanPrefab, zombiePrefab, PSGPrefab, obstaclePrefab;

	// Lists of all humans and zombies in the scene
	// The PSG that the humans will seek
	public List<GameObject> humans, zombies, obstacles;
	public GameObject PSG;

	// Floats for instantiating new humans/zombies
	public float humanMass, zombieMass;
	public float humanMaxSpeed, zombieMaxSpeed;

	// The number of humans/zombies that
	// will be spawned at the beginning of the run
	public int numOfHumans, numOfZombies, numOfObstacles;

	// The human/zombie that is being created
	GameObject newHuman, newZombie, newObstacle;

	// Other variables
	public bool isDebugging;
	public float bounds;

	// Start is called before the first frame update
    void Start()
    {
		humans = new List<GameObject>();
		zombies = new List<GameObject>();
		isDebugging = true;

		//// Creates 1 PSG for the Humans to seek
		//PSG = Instantiate(
		//	PSGPrefab,
		//	new Vector3(
		//		Random.Range(-bounds, bounds),
		//		PSGPrefab.GetComponent<MeshRenderer>().bounds.size.y / 2,
		//		Random.Range(-bounds, bounds)),
		//	Quaternion.identity);

		// Creates a specified number of humans when the scene is ran
		for(int i = 0; i < numOfHumans; i++)
		{
			newHuman = CreateHuman(
				new Vector3(
					Random.Range(-bounds, bounds),
					0.0f,
					Random.Range(-bounds, bounds)));
		}

		// Creates a specified number of zombies when the scene is ran
		for(int i = 0; i < numOfZombies; i++)
		{
			newZombie = CreateZombie(
				new Vector3(
					Random.Range(-bounds, bounds),
					0.0f,
					Random.Range(-bounds, bounds)));
		}

		//// Creates a specified number of obstacles when the scene is ran
		//for(int i = 0; i < numOfObstacles; i++)
		//{
		//	newObstacle = CreateObstacle(
		//		new Vector3(
		//			Random.Range(-bounds, bounds),
		//			obstaclePrefab.GetComponent<MeshRenderer>().bounds.size.y / 2,
		//			Random.Range(-bounds, bounds)));
		//}

		obstacles = new List<GameObject>(GameObject.FindGameObjectsWithTag("Obstacle"));
	}

    // Update is called once per frame
    void Update()
    {
		//PSGHumanCollision();
		HumanZombieCollision();

		// Toggles debug lines
		if(Input.GetKeyDown(KeyCode.D))
		{
			isDebugging = !isDebugging;
		}
	}

	/// <summary>
	/// Creates a human in the scene
	/// </summary>
	/// <param name="position">The position the human will be spawned</param>
	/// <param name="rotation">The rotation of the human when it is spawned</param>
	/// <returns>The human that is spawned into the scene</returns>
	public GameObject CreateHuman(Vector3 position, Quaternion rotation)
	{
		newHuman = Instantiate(
			humanPrefab,
			new Vector3(
				position.x,
				position.y,
				position.z),
			rotation);
		newHuman.GetComponent<Vehicle>().mass = humanMass;
		newHuman.GetComponent<Vehicle>().maxSpeed = humanMaxSpeed;
		newHuman.GetComponent<Human>().zombies = zombies;
		newHuman.GetComponent<Human>().PSG = PSG;
		humans.Add(newHuman);

		return newHuman;
	}

	/// <summary>
	/// Calls CreateHuman and sets the rotation to be Quaternion.identity
	/// </summary>
	/// <param name="position">The position that the human will be spawned at</param>
	/// <returns>The human that is created</returns>
	public GameObject CreateHuman(Vector3 position)
	{
		return CreateHuman(position, Quaternion.identity);
	}

	/// <summary>
	/// Creates a zombie in the scene
	/// </summary>
	/// <param name="position">The position the zombie will be spawned</param>
	/// <param name="rotation">The rotation of the zombie when it is spawned</param>
	/// <returns>The zombie that is spawned into the scene</returns>
	public GameObject CreateZombie(Vector3 position, Quaternion rotation)
	{
		newZombie = Instantiate(
			zombiePrefab,
			new Vector3(
				position.x,
				position.y,
				position.z),
			rotation);
		newZombie.GetComponent<Vehicle>().mass = zombieMass;
		newZombie.GetComponent<Vehicle>().maxSpeed = zombieMaxSpeed;
		newZombie.GetComponent<Zombie>().humans = humans;
		zombies.Add(newZombie);

		return newZombie;
	}

	/// <summary>
	/// Calls CreateZombie and sets the rotation to be Quaternion.identity
	/// </summary>
	/// <param name="position">The position that the zombie will be spawned at</param>
	/// <returns>The zombie that is created</returns>
	public GameObject CreateZombie(Vector3 position)
	{
		return CreateZombie(position, Quaternion.identity);
	}

	/// <summary>
	/// Creates a obstacle in the scene
	/// </summary>
	/// <param name="position">The position the obstacle will be spawned</param>
	/// <param name="rotation">The rotation of the obstacle when it is spawned</param>
	/// <returns>The obstacle that is spawned into the scene</returns>
	public GameObject CreateObstacle(Vector3 position, Quaternion rotation)
	{
		newObstacle = Instantiate(
			obstaclePrefab,
			new Vector3(
				position.x,
				position.y,
				position.z),
			rotation);
		obstacles.Add(newObstacle);

		return newObstacle;
	}

	/// <summary>
	/// Calls CreateObstacle and sets the rotation to be Quaternion.identity
	/// </summary>
	/// <param name="position">The position that the obstacle will be spawned at</param>
	/// <returns>The obstacle that is created</returns>
	public GameObject CreateObstacle(Vector3 position)
	{
		return CreateObstacle(position, Quaternion.identity);
	}

	/// <summary>
	/// Checks if any of the humans are colliding with the PSG, 
	/// if so, the PSG will teleport to a new location in the scene
	/// </summary>
	void PSGHumanCollision()
	{
		foreach(GameObject human in humans)
		{
			if(Vector3.Distance(human.transform.position, PSG.transform.position) < 1f)
			{
				PSG.transform.position = new Vector3(
					Random.Range(-bounds, bounds),
					PSGPrefab.GetComponent<MeshRenderer>().bounds.size.y / 2,
					Random.Range(-bounds, bounds));
			}
		}
	}

	/// <summary>
	/// Checks if any of the zombies are colliding with any humans
	/// if so, that human gameObject is Destroyed and a zombie
	/// gameObject is spawned in its place
	/// </summary>
	void HumanZombieCollision()
	{
		List<GameObject> collidedHumans = new List<GameObject>();

		foreach(GameObject human in humans)
		{
			foreach(GameObject zombie in zombies)
			{
				if(Vector3.Distance(human.transform.position, zombie.transform.position) < 1.5f)
				{
					// Adds the collided human to a list to be destroyed (see below)
					collidedHumans.Add(human);
				}
			}
		}

		foreach(GameObject human in collidedHumans)
		{
			newZombie = CreateZombie(
				new Vector3(
					human.transform.position.x,
					0.0f,
					human.transform.position.z),
				human.transform.rotation);
			zombies.Add(newZombie);
			humans.Remove(human);
			Destroy(human);
		}
	}
}