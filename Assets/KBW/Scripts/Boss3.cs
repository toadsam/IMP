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

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        StartCoroutine(SideRun());
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
            spawner.OnBoss3Slained();
            animator.SetBool("isDeath", true);
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
        float sideRange = 0.3f;        // 좌우 이동 폭

        Vector3 startPos = transform.position + new Vector3(0, 0, 0.1f);

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
