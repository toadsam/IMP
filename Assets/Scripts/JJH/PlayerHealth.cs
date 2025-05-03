using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public EndUI endUI;

    [SerializeField] private Slider hpSlider; // ✅ Unity UI 슬라이더 (Slider_Top 연결)

    void Start()
    {
        currentHealth = maxHealth;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망!");
        endUI.LossEnd();
        // 사망 시 처리
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
