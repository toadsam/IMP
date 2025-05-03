using System.Collections;
using UnityEngine;

public class MonsterSpawnerMover : MonoBehaviour
{
    public UseThis useThisScript;  // Reference to the UseThis script (assign it via Inspector)
    public GameObject monsterSpawner;  // Prefab for the monster spawner
    public GameObject monsterSpawnerInstance;  // Instantiated monster spawner object

    void Start()
    {
        // Start the coroutine to move the monster spawner once the game has started
        StartCoroutine(MoveWhenGameStarts());
    }

    IEnumerator MoveWhenGameStarts()
    {
        while (true)
        {
            // Only operate if UseThis is linked and the game has started
            if (useThisScript != null && useThisScript.isGameStart)
            {
                // Instantiate the monster spawner prefab once (only if not already created)
                if (monsterSpawnerInstance == null)
                    monsterSpawnerInstance = Instantiate(monsterSpawner, Vector3.zero, Quaternion.identity);

                Debug.Log("move spawner!");

                // Get the available spawn points from UseThis script
                Vector3[] points = useThisScript.spawnerPoint;

                if (points != null && points.Length > 0)
                {
                    // Pick a random position from the available points
                    int randomIndex = Random.Range(0, points.Length);
                    Vector3 targetPos = points[randomIndex];

                    Debug.Log($"Moving to: {targetPos}");

                    // Move the instantiated spawner to the chosen position
                    monsterSpawnerInstance.transform.position = targetPos;
                }
                else
                {
                    Debug.LogWarning("Spawner point array is empty or null.");
                }
            }

            // Wait for 1 second before next movement
            yield return new WaitForSeconds(1f);
        }
    }
}
