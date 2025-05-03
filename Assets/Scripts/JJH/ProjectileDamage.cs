using System.Threading;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public bool isDestroy;
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster")) // 태그가 Monster인 오브젝트와 충돌했을 때
        {
            Debug.Log($"몬스터에게 {damage} 데미지를 입혔습니다!");

            // 필요한 경우 여기에 실제 체력 감소 코드도 추가 가능
            if (other.TryGetComponent<Enemy1>(out var enemy1))
            {
                enemy1.OnDamage(damage);
            }

            if (other.TryGetComponent<Boss1>(out var boss1))
            {
                boss1.OnDamage(damage);
            }

            if (other.TryGetComponent<Enemy2>(out var enemy2))
            {
                enemy2.OnDamage(damage);
            }

            if (other.TryGetComponent<Boss2>(out var boss2))
            {
                boss2.OnDamage(damage);
            }

            if (other.TryGetComponent<Enemy3>(out var enemy3))
            {
                enemy3.OnDamage(damage);
            }

            if (other.TryGetComponent<Boss3>(out var boss3))
            {
                boss3.OnDamage(damage);
            }

            if (isDestroy)
            Destroy(gameObject); // 충돌 후 제거하고 싶다면
        }
    }
}
