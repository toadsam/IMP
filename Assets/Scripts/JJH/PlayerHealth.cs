using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;                // Maximum health value
    private int currentHealth;                 // Current health of the player

    public EndUI endUI;                        // Reference to the End UI for game over

    [SerializeField] private Slider hpSlider;  // ✅ Unity UI Slider for health bar (linked to Slider_Top)

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize the health bar UI
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }

    void Update()
    {
        // No per-frame logic is needed here currently
    }

    // Called when the player takes damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // Prevent negative values
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();  // Trigger death if health drops to zero
        }
    }

    // Called to heal the player
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    // Updates the health bar UI to reflect current health
    private void UpdateHealthUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHealth;
        }
    }

    // Called when the player's health reaches zero
    private void Die()
    {
        Debug.Log("플레이어 사망!");  // "Player has died!" in Korean
        endUI.LossEnd();              // Trigger loss screen

        // Additional death behavior can be added here
    }

    // Returns the current health value
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
