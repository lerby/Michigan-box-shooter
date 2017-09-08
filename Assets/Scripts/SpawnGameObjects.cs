using UnityEngine;
using System.Collections;

public class SpawnGameObjects : MonoBehaviour
{
    // public variables
    public float targetMinimumSpeed = 0.1f;
    public float targetMaximumSpeed = 1f;

    public float startSpeed = 0.5f;

    public float secondsBetweenSpawning = 0.1f;
	public float xMinRange = -25.0f;
	public float xMaxRange = 25.0f;
	public float yMinRange = 8.0f;
	public float yMaxRange = 25.0f;
	public float zMinRange = -25.0f;
	public float zMaxRange = 25.0f;
	// public GameObject[] spawnObjects; // what prefabs to spawn

	private float nextSpawnTime;
    private float currentSpeed;
    private int spawnRateSum;

    public static SpawnGameObjects spawner;

    [System.Serializable]
    public class SpawnObjectList
    {
        public GameObject objectToSpawn;
        public int spawnRatio;
    }
    public SpawnObjectList[] spawnObjects;

    // Use this for initialization
    void Start ()
	{
		// determine when to spawn the next object
		nextSpawnTime = Time.time+secondsBetweenSpawning;
        currentSpeed = startSpeed;
        TargetHit(0);

        spawnRateSum = 0;
        for (int i = 0; i < spawnObjects.Length; ++i)
            spawnRateSum += spawnObjects[i].spawnRatio;

        if (spawner == null)
            spawner = this.gameObject.GetComponent<SpawnGameObjects>();
    }

    public void TargetHit(float speedChange)
    {
        float newSpeed = currentSpeed + speedChange;
        
        if (newSpeed > targetMaximumSpeed)
            currentSpeed = targetMaximumSpeed;
        else if (newSpeed < targetMinimumSpeed)
            currentSpeed = targetMinimumSpeed;
        else
            currentSpeed = newSpeed;

        TargetMover.motionMagnitude = 0.5f * currentSpeed;
        TargetMover.spinSpeed = 360.0f * currentSpeed;
    }
	
	// Update is called once per frame
	void Update ()
	{
		// exit if there is a game manager and the game is over
		if (GameManager.gm) {
			if (GameManager.gm.gameIsOver)
				return;
		}

		// if time to spawn a new game object
		if (Time.time  >= nextSpawnTime) {
			// Spawn the game object through function below
			MakeThingToSpawn ();

			// determine the next time to spawn the object
			nextSpawnTime = Time.time+secondsBetweenSpawning;
		}	
	}

    void MakeThingToSpawn()
    {
        Vector3 spawnPosition;

        // get a random position between the specified ranges
        spawnPosition.x = Random.Range(xMinRange, xMaxRange);
        spawnPosition.y = Random.Range(yMinRange, yMaxRange);
        spawnPosition.z = Random.Range(zMinRange, zMaxRange);
                                                
        // determine which object to spawn
        // int objectToSpawn = Random.Range (0, spawnObjects.Length);
        int objectToSpawn = Random.Range(0, spawnRateSum);


        // actually spawn the game object
        GameObject spawnedObject = null;

        int currentSpawnRatio = 0;
        for (int i = 0; i < spawnObjects.Length; ++i)
        {
            currentSpawnRatio += spawnObjects[i].spawnRatio;
            if (objectToSpawn < currentSpawnRatio)
            {
                spawnedObject = Instantiate(spawnObjects[i].objectToSpawn, spawnPosition, transform.rotation) as GameObject;
                break;
            }
        }

        if (spawnedObject == null)
        {
            return;
        }

		// make the parent the spawner so hierarchy doesn't get super messy
		spawnedObject.transform.parent = gameObject.transform;

        int targetMoverType = Random.Range(0, 10);

        if (targetMoverType < 4)
            spawnedObject.GetComponent<TargetMover>().motionState = TargetMover.motionDirections.Vertical;
        else if (targetMoverType < 8)
            spawnedObject.GetComponent<TargetMover>().motionState = TargetMover.motionDirections.Horizontal;
        else
            spawnedObject.GetComponent<TargetMover>().motionState = TargetMover.motionDirections.Spin;





    }
}
