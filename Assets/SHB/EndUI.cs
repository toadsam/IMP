using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    public TextMeshProUGUI titleMessege;
    void Start()
    {
        this.gameObject.SetActive(false);
        titleMessege.text = "You Defeated All Monsters!";
    }

    void Update()
    {
        
    }
}
