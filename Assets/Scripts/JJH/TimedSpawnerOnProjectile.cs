using UnityEngine;

public class TimedSpawnerOnProjectile : MonoBehaviour
{
    public GameObject objectToSpawn; // 생성할 물체 (예: 폭발 이펙트 등)
    public float spawnDelay = 1f;    // 몇 초 뒤에 생성할지
    public float destroyAfter = 2f;  // 생성된 물체를 몇 초 뒤에 삭제할지

    public GameObject enemy1; // 적에게 데미지를 주기 위한 함수 불러오기(김병욱) 

    private bool hasSpawned = false; // 중복 생성 방지

    void Start()
    {
        Invoke(nameof(SpawnAtCurrentPosition), spawnDelay);
    }

    void SpawnAtCurrentPosition()
    {
        if (hasSpawned || objectToSpawn == null) return;

        hasSpawned = true;
        GameObject spawned = Instantiate(objectToSpawn, transform.position, transform.rotation);
        Destroy(spawned, destroyAfter);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("Attacking Enemy"); // 충돌 확인
            other.GetComponent<Enemy1>().OnDamage(1f); // 데미지 함수
            SpawnAtCurrentPosition(); // 몬스터에 닿았을 때도 생성
        }
    }
}
