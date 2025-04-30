using UnityEngine;

public class Monster : MonoBehaviour
{
    public float speed = 1.5f;
    public Transform target;
    public Spawner spawner;
    
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

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target);
        Vector3 movement = new Vector3(transform.position.x, 0, transform.position.z).normalized;
        float movementSpeed = movement.magnitude;

        if(movementSpeed > 0.1f)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
                  
    }

    // 테스트용: 충돌 시 제거
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Die();
        }
    }

    void Die()
    {
        if (spawner != null)
        {
            spawner.OnSmallMonsterKilled();
        }

        Destroy(gameObject);
    }
}
