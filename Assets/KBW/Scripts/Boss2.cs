using System.Collections;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    public float speed = 0.1f;             // Normal walking speed
    public float rageSpeed = 0.5f;         // Speed during rage charge
    public float health = 200f;            // Boss health
    public float damage = 100f;            // Damage dealt to player on collision

    public AudioSource bossDeath;          // Sound played on death
    public AudioSource bossRage;           // Sound played when entering rage mode

    private Transform target;              // Player target
    private Spawner spawner;               // Reference to spawner for callbacks

    Animator animator;                     // Animator for animation control

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Rage());            // Start the boss behavior loop
    }

    // Initialize target and spawner from Spawner script
    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    // Coroutine controlling rage/walk/charge loop
    IEnumerator Rage()
    {
        while (true)
        {
            transform.LookAt(target);

            // Idle phase
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", false);
            animator.SetBool("isRage", false);
            animator.SetBool("isIdle", true);

            yield return new WaitForSeconds(2f);

            // Walk phase
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalk", true);

            float walkTime = 0f;

            while (walkTime < 3f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                transform.LookAt(target);
                walkTime += Time.deltaTime;
                yield return null;
            }

            // Rage animation phase
            animator.SetBool("isWalk", false);
            animator.SetBool("isRage", true);

            bossRage.Play(); // Play rage sound

            yield return new WaitForSeconds(1.6f);

            // Charge phase
            animator.SetBool("isRage", false);
            animator.SetBool("isRun", true);

            float runTime = 0f;

            while (runTime < 2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, rageSpeed * Time.deltaTime);
                transform.LookAt(target);
                runTime += Time.deltaTime;
                yield return null;
            }

            // Back to idle
            animator.SetBool("isRun", false);
            animator.SetBool("isIdle", true);
        }
    }

    // Called when boss dies
    public void Die()
    {
        animator.SetBool("isDeath", true);
        bossDeath.Play();

        if (spawner != null)
        {
            spawner.OnBoss2Slained();  // Notify spawner
        }

        Destroy(gameObject, 1f);       // Destroy with short delay
    }

    // Called when boss takes damage
    public void OnDamage(float damage)
    {
        health -= damage;
        Debug.Log("Boss2 attacked:" + health);

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    // Damage player on contact
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
