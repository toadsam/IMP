using System.Collections;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    public float speed = 0.1f;
    public float rageSpeed = 0.5f;
    public float health = 200f;
    public float damage = 1000f; //공격하진 않고 플레이어에게 다가오면 즉사

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

    //임시로 데미지 주는 코드
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnDamage(100f);
        }
    }

    void FixedUpdate()
    {

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Die();
        }
    }

    public void Die()
    {
        animator.SetBool("isDeath", true);
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
}
