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

    public void AddHealth(int health)
    {
        health += 10;
        Debug.Log("Your Health:" + health);
    }

}
