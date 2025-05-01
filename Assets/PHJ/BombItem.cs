using UnityEngine;

public class BombItem : MonoBehaviour
{
    [SerializeField] private float radius = 10.0f; // 폭발 반경
    [SerializeField] private float power = 600.0f; // 폭발 힘
    [SerializeField] private float lift = 350.0f; // 폭발로 인한 상승력

    public void Execute()
    {
        // 폭발 위치
        Vector3 explosionPos = transform.position;

        // 폭발 반경 내의 모든 Collider 가져오기
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            // Rigidbody가 있는 물체 중 Tag가 'Monster'인 경우에만 충격 가하기
            if (hit.CompareTag("Monster") && hit.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                rb.AddExplosionForce(power, explosionPos, radius, lift);
                Debug.Log($"Hit: {hit.name} with explosion force.");
            }
            else {
                Debug.Log("No Monster..");
            }
        }        
    }
}
