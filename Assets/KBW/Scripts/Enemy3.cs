using System.Collections;
using UnityEngine;

public class Enemy3 : MonoBehaviour
{
    public float speed = 0.5f;         // Movement speed
    public float health = 5f;          // Health points
    public float damage = 5f;          // Damage dealt to the player

    private Transform target;          // Target (usually the player)
    private Spawner spawner;           // Reference to the spawner
    private Rigidbody rb;              // Rigidbody for physics

    AudioSource enemySound;            // Sound to play on death
    Animator animator;                 // Animator for animations

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        enemySound = GetComponent<AudioSource>();
    }

    // Called externally to initialize target and spawner reference
    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    void Update()
    {
        // Debug/test feature: kill the enemy when space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnDamage(100f);
        }
    }

    void FixedUpdate()
    {
        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);

        // Animation toggle based on movement
        Vector3 movement = new Vector3(transform.position.x, 0, transform.position.z).normalized;
        float movementSpeed = movement.magnitude;
        animator.SetBool("isRun", movementSpeed > 0.1f);
    }

    // Handle enemy death
    void Die()
    {
        animator.SetBool("isDeath", true);
        enemySound.Play();

        if (spawner != null)
        {
            spawner.OnEnemy3Slained();  // Notify spawner
        }

        Destroy(gameObject, 1f);         // Destroy enemy after delay
    }

    // Called when enemy takes damage
    public void OnDamage(float damage)
    {
        health -= damage;
        Debug.Log("Enemy3 attacked:" + health);
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    // Handle collision with player
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage); // Deal damage to player
                Debug.Log("Dealt damage to the player.");
            }
        }
    }
}
