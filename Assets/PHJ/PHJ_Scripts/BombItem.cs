using UnityEngine;

/// <summary>
/// Handles the logic for bomb explosion,
/// applying force to nearby monsters.
/// </summary>
public class BombItem : MonoBehaviour
{
    [SerializeField] private float radius = 10.0f;      // explosion radius
    [SerializeField] private float power = 600.0f;      // strength of the explosion
    [SerializeField] private float lift = 350.0f;       // upward modifier for explosion force

    public void Execute()
    {
        // determine the center point of the explosion
        Vector3 explosionPos = transform.position;

        // get all colliders within the specified radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            // only affect objects tagged "Monster" that have a Rigidbody
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (hit.CompareTag("Monster") && rb != null)
            {
                // apply explosion force to the rigidbody
                rb.AddExplosionForce(power, explosionPos, radius, lift);
                Debug.Log($"Hit: {hit.name} with explosion force.");
            }
            else
            {
                Debug.Log("No Monster detected or missing Rigidbody.");
            }
        }        
    }
}
