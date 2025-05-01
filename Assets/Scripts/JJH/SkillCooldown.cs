using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillCooldown : MonoBehaviour
{
    public GameObject[] fillImages; // Fill_1 ~ Fill_6
    public float chargeInterval = 1f; // 몇 초마다 하나씩 켤지 설정
    public float cooldownTime = 6f; // 전체 쿨타임 (자동계산용)

    private float timer = 0f;
    private int currentIndex = 0;
    private bool isCharging = false;
    public bool isSkillReady => currentIndex >= fillImages.Length;




    void Start()
    {
        ResetUI();
        StartCharge();
    }

    public void StartCharge()
    {
        isCharging = true;
        timer = 0f;
        currentIndex = 0;
        ResetUI();
    }

    void Update()
    {
        if (!isCharging || isSkillReady) return;

        timer += Time.deltaTime;
        if (timer >= chargeInterval && currentIndex < fillImages.Length)
        {
            fillImages[currentIndex].SetActive(true);
            currentIndex++;
            timer = 0f;
        }
    }

    public void UseSkill()
    {
        if (isSkillReady)
        {
            Debug.Log("🔥 스킬 사용!");
            StartCharge(); // 다시 쿨타임 시작
        }
    }

    void ResetUI()
    {
        foreach (var img in fillImages)
        {
            img.SetActive(false);
        }
    }
}
