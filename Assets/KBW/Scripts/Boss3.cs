using System.Collections;
using UnityEngine;

public class Boss3 : MonoBehaviour
{
    public float speed = 0.5f;               // Not directly used here but represents general speed
    public float health = 300f;              // Boss health
    public float damage = 15f;               // Damage dealt to player

    private Transform target;                // Target (usually the player)
    private Spawner spawner;                 // Reference to the spawner

    private Rigidbody rb;                    // Rigidbody for physics-based movement
    private Animator animator;               // Animator for side movement animations
    AudioSource bossDeath;                   // Sound played upon death

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        bossDeath = GetComponent<AudioSource>();
        StartCoroutine(SideRun());           // Begin side-to-side movement behavior
    }

    // Called externally by the Spawner to assign target and spawner
    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    // Handles boss death
    void Die()
    {
        animator.SetBool("isDeath", true);
        bossDeath.Play();

        if (spawner != null)
        {
            spawner.OnBoss3Slained();        // Notify the spawner
        }

        Destroy(gameObject, 1f);             // Destroy after short delay
    }

    // Applies damage to the boss and checks for death
    public void OnDamage(float damage)
    {
        health -= damage;
        Debug.Log("Boss3 attacked:" + health);

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    // Coroutine to move boss side-to-side while facing the player
    IEnumerator SideRun()
    {
        float moveSpeed = 0.3f;              // Speed of lateral movement
        float sideRange = 0.5f;              // Width of side movement

        Vector3 startPos = transform.position + new Vector3(0, 0, -0.2f);  // Initial position offset

        while (true)
        {
            if (target == null) yield break;

            transform.LookAt(target);

            // Calculate forward direction to player
            Vector3 forwardDir = (target.position - startPos).normalized;
            forwardDir.y = 0;

            // Determine lateral (right) direction
            Vector3 rightDir = Vector3.Cross(Vector3.up, forwardDir).normalized;

            // Calculate side movement offset using PingPong for back-and-forth motion
            float side = Mathf.PingPong(Time.time * moveSpeed, sideRange * 2) - sideRange;
            Vector3 offset = rightDir * side;

            // Maintain current Y-position for proper physics interaction
            Vector3 newPos = startPos + offset;
            newPos.y = rb.position.y;

            rb.MovePosition(newPos);         // Apply movement using Rigidbody

            // Set animation state based on direction
            animator.SetBool("isRight", side > 0.05f);
            animator.SetBool("isLeft", side < -0.05f);

            yield return new WaitForFixedUpdate();
        }
    }

    // Deal damage to player on collision
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);  // Apply damage
                Debug.Log("Dealt damage to the player.");
            }
        }
    }
}
