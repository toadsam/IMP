using System.Collections;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    public float speed = 0.25f;
    public float health = 100f;
    public float damage = 10f;

    private Transform target;
    private Spawner spawner;

    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    //임시로 데미지 주는 코드
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnDamage(5f);
        }
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);
        Vector3 movement = new Vector3(transform.position.x, 0, transform.position.z).normalized;
        float movementSpeed = movement.magnitude;

        if (movementSpeed > 0.1f)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Die();
        }
    }
    void Die()
    {
        if (spawner != null)
        {
            spawner.OnBoss1Slained();
        }

        Destroy(gameObject);
    }

    public void OnDamage(float damage)
    {
        health -= damage;
        Debug.Log("Enemy1 attacked:" + health);
        animator.SetTrigger("isHit");
        if (health <= 0)
        {
            Die();
        }
    }
}
