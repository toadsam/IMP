using System.Collections;
using UnityEngine;

/// <summary>
/// Handles timed explosion attack that affects nearby monsters after a delay.
/// </summary>
public class TimeAttack : MonoBehaviour
{
    [SerializeField] private AudioSource BombAudioSource; // Audio source for bomb explosion sound
    [SerializeField] private float radius = 30f;          // Explosion radius
    [SerializeField] private float power = 30f;           // Explosion force power
    [SerializeField] private float lift = 10f;            // Upward force applied during explosion

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Starts the bomb audio and explosion coroutine after a delay.
    /// </summary>
    void OnEnable()
    {
        // Start delayed explosion sequence
        StartCoroutine(PlayBombAudioAfterDelay(6f));
    }

    /// <summary>
    /// Coroutine that waits for a specified delay, then applies explosion force to monsters.
    /// </summary>
    private IEnumerator PlayBombAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 explosionPos = transform.position;

        // Find all GameObjects tagged as "Monster"
        var monsters = GameObject.FindGameObjectsWithTag("Monster");

        if (monsters.Length == 0)
        {
            Debug.Log("No monsters found with the 'Monster' tag.");
        }
        else
        {
            Debug.Log($"Found {monsters.Length} GameObjects with the 'Monster' tag:");
            foreach (var monster in monsters)
            {
                Debug.Log($"Monster: {monster.name}");
            }
        }

        // Apply explosion force to each monster with Rigidbody within range
        foreach (var monster in monsters)
        {
            var rb = monster.GetComponent<Rigidbody>();

            // Skip monsters without Rigidbody
            if (rb == null)
            {
                Debug.Log($"Monster {monster.name} does not have a Rigidbody.");
                continue;
            }

            // Check if monster is within explosion radius
            float dist = Vector3.Distance(explosionPos, monster.transform.position);
            if (dist > radius)
            {
                Debug.Log($"Monster {monster.name} is outside the explosion radius.");
                Debug.Log($"Distance: {dist}, Radius: {radius}");
                continue;
            }

            // Apply explosion force with upward lift
            rb.AddExplosionForce(
                power,
                explosionPos,
                radius,
                lift,
                ForceMode.Impulse
            );
            Debug.Log($"Explosion force applied to {monster.name}.");
        }

        // Play bomb sound if available
        if (BombAudioSource != null)
        {
            BombAudioSource.Play();
            Debug.Log("Bomb audio played.");
        }
        else
        {
            Debug.LogWarning("BombAudioSource is not assigned!");
        }
    }
}
