using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private Boss2 boss2;
    void Start()
    {
        boss2 = GetComponentInParent<Boss2>();  
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            boss2.Die();
        }
    }
}


