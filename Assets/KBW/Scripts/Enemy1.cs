using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public float speed = 0.5f;         // Movement speed of the enemy
    public float health = 10f;         // Health points
    public float damage = 1f;          // Damage dealt to the player
    private Transform target;          // Target to chase (usually the player)
    private Spawner spawner;           // Reference to the spawner for callbacks

    AudioSource enemySound;            // Audio source for death sound
    Animator animator;                 // Animator for movement and death animations

    void Start()
    {
        // Get Animator and AudioSource from child components
        animator = GetComponentInChildren<Animator>();
        enemySound = GetComponentInChildren<AudioSource>();
    }

    // Called externally to initialize target and spawner
    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    void FixedUpdate()
    {
        // Move toward the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Face the target
        transform.LookAt(target);

        // Calculate movement speed for animation
        Vector3 movement = new Vector3(transform.position.x, 0, transform.position.z).normalized;
        float movementSpeed = movement.magnitude;

        // Set running animation based on movement
        if (movementSpeed > 0.1f)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
    }

    // Called when the enemy takes damage
    public void OnDamage(float damage)
    {
        health -= damage;
        Debug.Log("Enemy1 attacked:" + health);

        // If health drops to zero or below, trigger death
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    // Handle enemy death
    void Die()
    {
        animator.SetBool("isDeath", true);  // Trigger death animation
        enemySound.Play();                  // Play death sound

        if (spawner != null)
        {
            spawner.OnEnemy1Slained();      // Notify spawner that this enemy was killed
        }

        Destroy(gameObject, 1f);            // Destroy enemy object after delay
    }

    // Handle collision with player
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage); // Deal damage to the player
                Debug.Log("Dealt damage to the player.");
            }
        }
    }
}
