using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1f;        // Speed of the projectile
    public float lifetime = 5f;     // Lifetime before the bullet auto-destroys
    public float damage = 5f;       // Damage dealt to the player

    private Transform target;       // Target to follow (usually the player)

    void Start()
    {
        // Automatically destroy the bullet after 'lifetime' seconds
        Destroy(gameObject, lifetime);

        // Find and assign the player (in this case, the Main Camera)
        target = GameObject.Find("Main Camera").transform;
    }

    void Update()
    {
        // If the target no longer exists, destroy the bullet
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move the bullet toward the target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // If the bullet hits the player
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroy the bullet

            // Apply damage if the player has a PlayerHealth component
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
                Debug.Log("Dealt damage to the player.");
            }
        }
    }
}
