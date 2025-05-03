using UnityEngine;

public class Player : MonoBehaviour
{
        
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Add Player health about 10.
    public void AddHealth(int health)
    {
        health += 10;
        Debug.Log("Your Health:" + health);
    }

}
