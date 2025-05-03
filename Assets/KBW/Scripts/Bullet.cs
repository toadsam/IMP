using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1f;        // 투사체 속도
    public float lifetime = 5f;
    public float damage = 5f;

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifetime); // 일정 시간 후 자동 삭제
        target = GameObject.Find("Main Camera").transform;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage); // 데미지 적용
                Debug.Log("플레이어에게 데미지를 주었습니다.");
            }
        }
    }
}
