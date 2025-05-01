using System.Collections;
using UnityEngine;

public class TimedAttack : MonoBehaviour
{
    [SerializeField] private AudioSource BombAudioSource;
    [SerializeField] private float radius = 5f; // 폭발 반경
    [SerializeField] private float power = 500f; // 폭발 힘
    [SerializeField] private float lift = 10f; // 폭발로 인한 상승력

    void OnEnable()
    {
        // Play BombAudioSource after 6 seconds
        StartCoroutine(PlayBombAudioAfterDelay(6f));
    }

    private IEnumerator PlayBombAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Vector3 explosionPos = transform.position;

        // 폭발 반경 내의 모든 Collider 가져오기
        Collider[] hits = Physics.OverlapSphere(explosionPos, radius);
        
        foreach (var hit in hits)
        {
            // 3) 태그가 "Monster"인 오브젝트만 처리
            if (hit.CompareTag("Monster"))
            {
                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    // 5) 폭발력 적용
                    rb.AddExplosionForce(
                        power,      // 힘 크기
                        explosionPos,        // 폭발 중심
                        radius,     // 반경
                        lift,      // 위로 솟구치는 추가 힘
                        ForceMode.Impulse    // 즉시 효과
                    );
                }
            }
            else
            {
                Debug.Log($"Hit: {hit.name} is not a monster.");
            }
        }

        if (BombAudioSource != null)
        {
            BombAudioSource.Play();
            Debug.Log("Bomb audio played.");
        }
        else
        {
            Debug.LogWarning("BombAudioSource is not assigned!");
        }
    }
}


