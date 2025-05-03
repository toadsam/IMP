using System.Collections;
using UnityEngine;

public enum Enemy3State { Normal, Dragged, Thrown } //Enemy3 던지기 기능을 위한 상태들

public class Enemy3 : MonoBehaviour
{
    public float speed = 0.5f;
    public float health = 5f;
    public float damage = 5f;

    private Transform target;
    private Spawner spawner;
    private Rigidbody rb;

    AudioSource enemySound;
    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        enemySound = GetComponent<AudioSource>();
    }

    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnDamage(100f);
        }
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);
        Vector3 movement = new Vector3(transform.position.x, 0, transform.position.z).normalized;
        float movementSpeed = movement.magnitude;
        animator.SetBool("isRun", movementSpeed > 0.1f);
    }

    void Die()
    {
        animator.SetBool("isDeath", true);
        enemySound.Play();
        if (spawner != null)
        {
            spawner.OnEnemy3Slained();
        }

        Destroy(gameObject,1f);
    }

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

}
