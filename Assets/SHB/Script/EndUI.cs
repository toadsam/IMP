using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndUI : MonoBehaviour
{
    public TextMeshProUGUI titleMessege;

    public GameObject endUIChild;

    void Start()
    {
        // Set default ending message when the game ends (if no specific outcome is set)
        titleMessege.text = "You Defeated All Monsters!";
    }

    // Called when the player loses the game
    public void LossEnd()
    {
        endUIChild.SetActive(true);              // Show end screen panel
        titleMessege.text = "You Die!";          // Display loss message
        Time.timeScale = 1.0f;                   // Resume game time (if it was paused)
    }

    // Called when the player wins the game
    public void WinEnd()
    {
        endUIChild.SetActive(true);              // Show end screen panel
        // Time.timeScale = 1.0f;                // (Optional) Resume game time if needed
    }
}
