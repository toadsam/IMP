using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillCooldown : MonoBehaviour
{
    public GameObject[] fillImages;         // Array of UI indicators (e.g., Fill_1 to Fill_6)
    public float chargeInterval = 1f;       // Time interval between each fill indicator activation
    public float cooldownTime = 6f;         // Total cooldown time (used for reference)

    private float timer = 0f;               // Timer to track intervals
    private int currentIndex = 0;           // Index of the next fill image to activate
    private bool isCharging = false;        // Whether cooldown is in progress

    // Property that checks if the skill is fully charged and ready to use
    public bool isSkillReady => currentIndex >= fillImages.Length;

    void Start()
    {
        ResetUI();       // Turn off all fill indicators
        StartCharge();   // Begin cooldown at start
    }

    // Starts the cooldown charge process
    public void StartCharge()
    {
        isCharging = true;
        timer = 0f;
        currentIndex = 0;
        ResetUI();       // Hide all fill UI
    }

    void Update()
    {
        // Skip update if not charging or already fully charged
        if (!isCharging || isSkillReady) return;

        timer += Time.deltaTime;

        // Activate the next fill indicator after interval
        if (timer >= chargeInterval && currentIndex < fillImages.Length)
        {
            fillImages[currentIndex].SetActive(true);
            currentIndex++;
            timer = 0f;
        }
    }

    // Call this when the player uses the skill
    public void UseSkill()
    {
        if (isSkillReady)
        {
            Debug.Log("🔥 Skill Used!");
            StartCharge();  // Restart cooldown after use
        }
    }

    // Resets all fill indicators to hidden
    void ResetUI()
    {
        foreach (var img in fillImages)
        {
            img.SetActive(false);
        }
    }
}
