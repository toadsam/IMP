using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public float speed = 0.5f;
    public float health = 10f;
    public float damage = 1f;
    private Transform target;
    private Spawner spawner;

    AudioSource enemySound;
    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        enemySound = GetComponentInChildren<AudioSource>();
    }

    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);
        Vector3 movement = new Vector3(transform.position.x, 0, transform.position.z).normalized;
        float movementSpeed = movement.magnitude;

        if(movementSpeed > 0.1f)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
                  
    }

    public void OnDamage(float damage)
    {
        health -= damage;
        Debug.Log("Enemy1 attacked:" + health);
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    void Die()
    {
        animator.SetBool("isDeath", true);
        enemySound.Play();
        if (spawner != null)
        {
            spawner.OnEnemy1Slained();
        }
        Destroy(gameObject, 1f);
    }
}
