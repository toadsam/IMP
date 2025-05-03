using System.Collections;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public float speed = 0.5f;
    public float health = 15f;
    public float damage = 1.5f;

    private Transform target;
    private Spawner spawner;

    AudioSource enemySound;
    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        enemySound = GetComponentInChildren<AudioSource>();
        StartCoroutine(Run());
    }

    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    IEnumerator Run()
    {
        while (true)
        {
            transform.LookAt(target);
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

    void Die()
    {
        animator.SetBool("isDeath", true);
        enemySound.Play();
        if (spawner != null)
        {
            spawner.OnEnemy2Slained();
        }
        Destroy(gameObject,1f);
    }

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage); // 데미지 적용
                Debug.Log("플레이어에게 데미지를 주었습니다.");
            }
        }
    }
}
