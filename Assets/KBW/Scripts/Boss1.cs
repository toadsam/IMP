using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Boss1 : MonoBehaviour
{
    public float speed = 0.25f;
    public float health = 100f;
    public float damage = 10f;
    public GameObject bullet1;

    public AudioSource bossDeath;
    public AudioSource shoot;

    private Transform target;
    private Spawner spawner;

    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Attack());
    }

    public void Init(Transform target, Spawner spawner)
    {
        this.target = target;
        this.spawner = spawner;
    }

    void Die()
    {
        animator.SetBool("isDeath", true);
        bossDeath.Play();
        if (spawner != null)
        {
            spawner.OnBoss1Slained();
        }
        Destroy(gameObject, 2f);
    }

    public void OnDamage(float damage)
    {
        health -= damage;
        Debug.Log("Boss1 attacked:" + health);
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            transform.LookAt(target);

            animator.SetBool("isRun", false);
            animator.SetBool("isAttack", true);

            yield return new WaitForSeconds(2.5f);

            if (bullet1 != null && target != null)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(dir);

                Vector3 offset = new Vector3(-0.05f, 0.05f, 0);

                shoot.Play();   
                Instantiate(bullet1, transform.position + offset, rotation);
            }

            animator.SetBool("isAttack", false);
            animator.SetBool("isRun", true);

            float runTime = 0f;

            while (runTime < 5f)
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
