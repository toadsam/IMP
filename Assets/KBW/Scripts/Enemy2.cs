using System.Collections;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public float speed = 0.5f;           // Movement speed
    public float health = 15f;           // Health points
    public float damage = 1.5f;          // Damage dealt to the player

    private Transform target;            // Player target
    private Spawner spawner;             // Reference to spawner

    AudioSource enemySound;              // Death sound effect
    Animator animator;                   // Animator for movement and death

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        enemySound = GetComponentInChildren<AudioSource>();
        StartCoroutine(Run());           // Start enemy movement pattern
    }

    // Initialization from Spawner
    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    // Coroutine to control enemy's movement behavior (pause-run pattern)
    IEnumerator Run()
    {
        while (true)
        {
            // Pause and face the target
            transform.LookAt(target);
            animator.SetBool("isRun", false);
            yield return new WaitForSeconds(1.5f);

            // Run toward the target for 1 second
            animator.SetBool("isRun", true);
            float runTime = 0f;

            while (runTime < 1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                transform.LookAt(target);
                runTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    // Called when enemy dies
    void Die()
    {
        animator.SetBool("isDeath", true);  // Play death animation
        enemySound.Play();                  // Play sound

        if (spawner != null)
        {
            spawner.OnEnemy2Slained();      // Notify spawner
        }

        Destroy(gameObject, 1f);             // Destroy after delay
    }

    // Called when enemy takes damage
    public void OnDamage(float damage)
    {
        health -= damage;
        Debug.Log("Enemy2 attacked:" + health);
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
                playerHealth.TakeDamage((int)damage); // Apply damage to player
                Debug.Log("Dealt damage to the player.");
            }
        }
    }
}
