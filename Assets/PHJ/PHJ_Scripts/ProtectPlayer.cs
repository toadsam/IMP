using UnityEngine;

/// <summary>
/// Protects the player by blocking and handling incoming projectiles or enemies.
/// </summary>
public class ProtectPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource blockSound; // Sound played when blocking a projectile

    private float damage = 50f; // Damage dealt to enemy projectiles upon blocking

    /// <summary>
    /// Called when another collider enters this trigger collider.
    /// Handles blocking of monsters and bullets.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the incoming object is tagged as "Monster"
        if (other.CompareTag("Monster"))
        {
            Debug.Log($"Monster {other.name} was blocked by the shield.");
            blockSound.Play();

            // Check for each possible enemy type and apply damage if applicable
            if (other.TryGetComponent<Enemy1>(out var enemy1))
            {
                enemy1.OnDamage(damage);
            }

            if (other.TryGetComponent<Boss1>(out var boss1))
            {
                boss1.OnDamage(damage);
            }

            if (other.TryGetComponent<Enemy2>(out var enemy2))
            {
                enemy2.OnDamage(damage);
            }

            if (other.TryGetComponent<Boss2>(out var boss2))
            {
                boss2.OnDamage(damage);
            }

            if (other.TryGetComponent<Enemy3>(out var enemy3))
            {
                enemy3.OnDamage(damage);
            }

            if (other.TryGetComponent<Boss3>(out var boss3))
            {
                boss3.OnDamage(damage);
            }
        }

        // Check if the incoming object is tagged as "Bullet"
        if (other.CompareTag("Bullet"))
        {
            Debug.Log($"Bullet {other.name} was blocked by the shield.");

            // Play block sound and destroy bullet
            blockSound.Play();
            Destroy(other.gameObject);
        }
    }
}