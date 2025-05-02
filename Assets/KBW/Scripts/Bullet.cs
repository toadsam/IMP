using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;        // 투사체 속도
    public float lifetime = 5f; 

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifetime); // 일정 시간 후 자동 삭제
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
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
            // 플레이어에게 데미지를 주는 함수 호출
            Destroy(gameObject); 
        }
    }
}
