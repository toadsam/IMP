using System.Collections;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public float speed = 0.5f;
    public float health = 15f;
    public float damage = 1.5f;

    private Transform target;
    private Spawner spawner;

    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Run());
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
       
    }

    IEnumerator Run()
    {
        while (true)
        {
            animator.SetBool("isRun", false);
            yield return new WaitForSeconds(1.5f);

            animator.SetBool("isRun", true);
            float runTime = 0f;

            while(runTime < 1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                transform.LookAt(target);
                runTime += Time.deltaTime;
                yield return null;
            }            
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
            spawner.OnEnemy2Slained();
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
