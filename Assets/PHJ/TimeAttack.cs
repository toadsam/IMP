using System.Collections;
using UnityEngine;

public class TimeAttack : MonoBehaviour
{
    [SerializeField] private AudioSource BombAudioSource;
    [SerializeField] private float radius = 30f; // 폭발 반경
    [SerializeField] private float power = 30f; // 폭발 힘
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

        // 'Monster' 태그를 가진 모든 GameObject 가져오기
        var monsters = GameObject.FindGameObjectsWithTag("Monster");

        if (monsters.Length == 0)
        {
            Debug.Log("No monsters found with the 'Monster' tag.");
        }
        else
        {
            Debug.Log($"Found {monsters.Length} GameObjects with the 'Monster' tag:");
            foreach (var monster in monsters)
            {
                Debug.Log($"Monster: {monster.name}");
            }
        }

        foreach (var monster in monsters)
        {
            // Rigidbody가 있는지 검사
            var rb = monster.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.Log($"Monster {monster.name} does not have a Rigidbody.");
                continue;
            }

            // 폭발 반경 내에 있는지 거리로 검사
            float dist = Vector3.Distance(explosionPos, monster.transform.position);
            if (dist > radius)
            {
                Debug.Log($"Monster {monster.name} is outside the explosion radius.");
                Debug.Log($"Distance: {dist}, Radius: {radius}");
                continue;
            }

            // AddExplosionForce로 밀어내기
            rb.AddExplosionForce(
                power,
                explosionPos,
                radius,
                lift,
                ForceMode.Impulse
            );
            Debug.Log($"Explosion force applied to {monster.name}.");
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
