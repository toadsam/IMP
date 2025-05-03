using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Boss1 : MonoBehaviour
{
    public float speed = 0.25f;             // Movement speed
    public float health = 100f;             // Boss health
    public float damage = 10f;              // Damage to player
    public GameObject bullet1;              // Bullet prefab for ranged attack

    public AudioSource bossDeath;           // Sound played on death
    public AudioSource shoot;               // Sound played when shooting

    private Transform target;               // Target (usually the player)
    private Spawner spawner;                // Reference to the spawner for callbacks

    Animator animator;                      // Animator for attack/run/death

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Attack());           // Begin attack behavior loop
    }

    // Initialize the boss with the player's transform and the spawner reference
    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    // Handle death logic
    void Die()
    {
        animator.SetBool("isDeath", true);  // Play death animation
        bossDeath.Play();                   // Play death sound

        if (spawner != null)
        {
            spawner.OnBoss1Slained();       // Notify spawner that boss is dead
        }

        Destroy(gameObject, 2f);            // Remove object after delay
    }

    // Handle incoming damage
    public void OnDamage(float damage)
    {
        health -= damage;
        Debug.Log("Boss1 attacked:" + health);

        if (health <= 0)
        {
            health = 0;
            Die();                          // Trigger death if health depleted
        }
    }

    // Boss attack and movement pattern
    IEnumerator Attack()
    {
        while (true)
        {
            transform.LookAt(target);       // Always face the player

            animator.SetBool("isRun", false);
            animator.SetBool("isAttack", true);

            yield return new WaitForSeconds(2.5f);  // Prepare attack

            // Shoot bullet toward the player
            if (bullet1 != null && target != null)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(dir);

                Vector3 offset = new Vector3(-0.05f, 0.05f, 0);  // Slight offset to spawn bullet

                shoot.Play();
                Instantiate(bullet1, transform.position + offset, rotation);
            }

            animator.SetBool("isAttack", false);
            animator.SetBool("isRun", true);

            float runTime = 0f;

            // Run toward the player for a duration
            while (runTime < 5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                transform.LookAt(target);
                runTime += Time.deltaTime;
                yield return null;
            }
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
                playerHealth.TakeDamage((int)damage);  // Apply damage to player
                Debug.Log("Dealt damage to the player.");
            }
        }
    }
}
