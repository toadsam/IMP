using UnityEngine;

public class ProtectPlayer : MonoBehaviour
{    
    [SerializeField] private AudioSource blockSound;
    // 투사체를 감지하고 제거하는 기능
    private void OnTriggerEnter(Collider other)
    {
        // 투사체의 Tag가 "Projectile"인지 확인
        if (other.CompareTag("Monster"))
        {
            Debug.Log($"Monster {other.name} was blocked by the shield.");
            
            // 투사체 제거
            blockSound.Play();
            Destroy(other.gameObject);
        }
    }
}
