using System.Collections;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    public float speed = 0.1f;
    public float rageSpeed = 0.5f;
    public float health = 200f;
    public float damage = 100f;

    public AudioSource bossDeath;
    public AudioSource bossRage;

    private Transform target;
    private Spawner spawner;

    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Rage());        
    }

    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    IEnumerator Rage()
    {
        while (true)
        {
            transform.LookAt(target);

            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", false);
            animator.SetBool("isRage", false);
            animator.SetBool("isIdle", true);

            yield return new WaitForSeconds(2f);

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

            animator.SetBool("isWalk", false);
            animator.SetBool("isRage", true);

            bossRage.Play();

            yield return new WaitForSeconds(1.6f);

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

            animator.SetBool("isRun", false);
            animator.SetBool("isIdle", true);
            
        }
    }
    public void Die()
    {
        animator.SetBool("isDeath", true);
        bossDeath.Play();

        if (spawner != null)
        {
            spawner.OnBoss2Slained();
        }
        Destroy(gameObject, 1f);
    }

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

        if (other.CompareTag("Shield"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 pushDirection = (transform.position - transform.position).normalized;
                    float pushForce = 5f; // 밀어내는 힘의 크기 조절 가능
                    rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                }
            
        }
    }
}
