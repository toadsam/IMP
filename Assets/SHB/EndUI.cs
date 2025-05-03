using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndUI : MonoBehaviour
{
    public TextMeshProUGUI titleMessege;

    public GameObject endUIChild;
    void Start()
    {
        //this.gameObject.SetActive(false);
        titleMessege.text = "You Defeated All Monsters!";
    }

    void Update()
    {
        
    }

    public void LossEnd() 
    {
        endUIChild.SetActive(true);
        titleMessege.text = "You Die!";
        Time.timeScale = 1.0f;
    }
    public void WinEnd()
    {
        endUIChild.SetActive(true);      //  Time.timeScale = 1.0f;
    }



}
