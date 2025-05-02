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
    private Rigidbody rb; //rigid body는 언제든 쓰일 수 있으니 놔두기
    Animator animator;


    //Enemy3를 던지기 위한 초기 상태 설정 및 변수
    //public Enemy3State state = Enemy3State.Normal;
    //public Vector3 dragTargetPos;
    //public float throwPower = 5f;

    //[HideInInspector] public Coroutine cloakingCoroutine;
    //public bool isBeingDragged = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
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

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);
        Vector3 movement = new Vector3(transform.position.x, 0, transform.position.z).normalized;
        float movementSpeed = movement.magnitude;
        animator.SetBool("isRun", movementSpeed > 0.1f);

        /*if (state == Enemy3State.Normal)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            transform.LookAt(target);
            Vector3 movement = new Vector3(transform.position.x, 0, transform.position.z).normalized;
            float movementSpeed = movement.magnitude;
            animator.SetBool("isRun", movementSpeed > 0.1f);
        }
        else if (state == Enemy3State.Dragged)
        {
            Vector3 worldPos = dragTargetPos;
            worldPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, worldPos, 0.2f);
            transform.LookAt(worldPos);
            animator.SetBool("isDrag", false);
        }*/
    }

    /*public void Throw(Vector3 direction)
    {
        state = Enemy3State.Thrown;
        rb.isKinematic = false;
        rb.AddForce(direction * throwPower, ForceMode.Impulse);
        animator.SetBool("isRun", false);
    }*/

    void OnTriggerEnter(Collider other)
    {
        /*if (state == Enemy3State.Thrown && other.CompareTag("Boss"))
        {
            // 보스에게 큰 데미지
            if (other.TryGetComponent<Boss3>(out var boss))
            {
                boss.OnDamage(100f);
            }

            Destroy(gameObject); // 충돌 후 삭제
        }*/

        if (other.CompareTag("Bullet"))
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetBool("isDeath", true);
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
