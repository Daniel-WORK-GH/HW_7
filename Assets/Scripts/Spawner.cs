using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab to spawn.")]
    public GameObject objectToSpawn;

    [Tooltip("Spawn radius from the center point.")]
    public float spawnRadius = 10f;

    [Tooltip("Height at which objects are spawned.")]
    public float spawnHeight = 20f;

    [Tooltip("Interval between spawns in seconds.")]
    public float spawnInterval = 1f;

    [Tooltip("Should the spawner start automatically?")]
    public bool autoStart = true;

    private float timer;

    void Start()
    {
        timer = spawnInterval;

        if (autoStart)
        {
            StartSpawning();
        }
    }

    void Update()
    {
        if (objectToSpawn == null) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnObject();
            timer = spawnInterval;
        }
    }

    public void StartSpawning()
    {
        enabled = true;
    }

    public void StopSpawning()
    {
        enabled = false;
    }

    private void SpawnObject()
    {
        Vector3 spawnPosition = GetRandomPosition();
        Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomPosition()
    {
        // Generate a random position within the spawn radius
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius + new Vector2(transform.position.x, transform.position.z);
        return new Vector3(randomPoint.x, spawnHeight, randomPoint.y);
    }
}
