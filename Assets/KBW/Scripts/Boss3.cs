using System.Collections;
using UnityEngine;

public class Boss3 : MonoBehaviour
{
    public float speed = 0.5f;
    public float health = 300f;
    public float damage = 15f;

    private Transform target;
    private Spawner spawner;

    private Rigidbody rb;
    private Animator animator;
    AudioSource bossDeath;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        bossDeath = GetComponent<AudioSource>();
        StartCoroutine(SideRun());
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
            spawner.OnBoss3Slained();
        }

        Destroy(gameObject, 1f);
    }

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

    IEnumerator SideRun()
    {
        float moveSpeed = 0.3f;       // 왕복 속도
        float sideRange = 0.5f;        // 좌우 이동 폭

        Vector3 startPos = transform.position + new Vector3(0, 0, -0.2f);

        while (true)
        {
            if (target == null) yield break;

            transform.LookAt(target);

            // 기준 방향 계산
            Vector3 forwardDir = (target.position - startPos).normalized;
            forwardDir.y = 0;
            Vector3 rightDir = Vector3.Cross(Vector3.up, forwardDir).normalized;

            // 좌우 위치 계산
            float side = Mathf.PingPong(Time.time * moveSpeed, sideRange * 2) - sideRange;
            Vector3 offset = rightDir * side;

            // 현재 Y 위치 유지 (중력 적용)
            Vector3 newPos = startPos + offset;
            newPos.y = rb.position.y;

            rb.MovePosition(newPos); // 물리 기반 이동

            // 애니메이션 방향 판별
            animator.SetBool("isRight", side > 0.05f);
            animator.SetBool("isLeft", side < -0.05f);

            yield return new WaitForFixedUpdate();
        }
    }



}
