using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster")) // 태그가 Monster인 오브젝트와 충돌했을 때
        {
            Debug.Log($"몬스터에게 {damage} 데미지를 입혔습니다!");

            // 필요한 경우 여기에 실제 체력 감소 코드도 추가 가능
            // other.GetComponent<MonsterHealth>()?.TakeDamage(damage);

            Destroy(gameObject); // 충돌 후 제거하고 싶다면
        }
    }
}
