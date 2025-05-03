using System.Threading;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public bool isDestroy;             // Determines whether the projectile should be destroyed after hitting a target
    public int damage = 10;            // Amount of damage the projectile deals

    private void OnTriggerEnter(Collider other)
    {
        // When the projectile collides with an object tagged "Monster"
        if (other.CompareTag("Monster"))
        {
            Debug.Log($"몬스터에게 {damage} 데미지를 입혔습니다!");  // "Dealt damage to monster!" in Korean

            // Try applying damage to all enemy types if the component is present
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

            // Destroy the projectile if the flag is set to true
            if (isDestroy)
                Destroy(gameObject);
        }
    }
}
