using System.Collections;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    public float speed = 0.01f;
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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnDamage(5f);
        }
    }

    void FixedUpdate()
    {

    }

    IEnumerator Rage()
    {
        while (true)
        {
            animator.SetBool("isWalk", false);
            animator.SetBool("isRage", true);
            yield return new WaitForSeconds(1.5f);

            animator.SetBool("isRage", false);
            animator.SetBool("isWalk", true);
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
        if (other.CompareTag("Bullet"))
        {
            Die();
        }
    }

    public void Die()
    {
        if (spawner != null)
        {
            spawner.OnBoss2Slained();
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
