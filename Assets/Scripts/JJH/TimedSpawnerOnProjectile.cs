using UnityEngine;

public class TimedSpawnerOnProjectile : MonoBehaviour
{
    public GameObject objectToSpawn;    // The object to spawn (e.g., explosion effect)
    public float spawnDelay = 1f;       // Delay in seconds before spawning the object
    public float destroyAfter = 2f;     // How long to wait before destroying the spawned object
    private bool hasSpawned = false;    // Prevents spawning more than once

    void Start()
    {
        // Automatically attempt to spawn after a delay
        Invoke(nameof(SpawnAtCurrentPosition), spawnDelay);
    }

    // Spawns the object at the projectile's current position
    void SpawnAtCurrentPosition()
    {
        // Do not spawn if already spawned or if no object is assigned
        if (hasSpawned || objectToSpawn == null) return;

        hasSpawned = true;

        // Instantiate the object at this position and rotation
        GameObject spawned = Instantiate(objectToSpawn, transform.position, transform.rotation);

        // Destroy the spawned object after a delay
        Destroy(spawned, destroyAfter);
    }

    // Also trigger spawn immediately on collision with a monster
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("Attacking Enemy"); // Confirm collision
            SpawnAtCurrentPosition();     // Trigger spawn effect on hit
        }
    }
}
