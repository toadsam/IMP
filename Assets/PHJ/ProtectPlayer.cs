using UnityEngine;

public class ProtectPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource blockSound;
    // 투사체를 감지하고 제거하는 기능

    private float damage = 50f;
    private void OnTriggerEnter(Collider other)
    {
        // 투사체의 Tag가 "Monster"인지 확인
        if (other.CompareTag("Monster"))
        {
            Debug.Log($"Monster {other.name} was blocked by the shield.");
            blockSound.Play();

            // 투사체 제거
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
        }


        if (other.CompareTag("Bullet"))
        {
            Debug.Log($"Bullet {other.name} was blocked by the shield.");

            // 투사체 제거
            blockSound.Play();
            Destroy(other.gameObject);
        }
    }
}