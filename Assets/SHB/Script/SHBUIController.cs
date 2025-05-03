using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SHBUIController : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject scanningScreen;
    public GameObject settingUI;
    public GameObject settingScreen;
    public GameObject fightLabel;
    public GameObject fightButton;
    public TextMeshProUGUI scanButtonText;
    public GameObject usethis;
    private bool isScanning = false;
    private bool isSettingOpen = false;

    void Start()
    {
        scanningScreen.SetActive(false);
        fightLabel.SetActive(false);
        fightButton.SetActive(false);
        settingScreen.SetActive(false);
    }

    public void SwitchScreens()
    {
        // startScreen을 끄고 scanningScreen을 켬
        if (startScreen != null && scanningScreen != null)
        {
            startScreen.SetActive(false);
            scanningScreen.SetActive(true);
        }
    }

    public void changeScanButtonText()
    {
        if (isScanning)
        {
            scanButtonText.text = "Restart Scan";
            isScanning = false;
            fightButton.SetActive(true);
            fightLabel.SetActive(true);
        }
        else
        {
            scanButtonText.text = "Stop Scan";
            isScanning = true;
            fightButton.SetActive(false);
            fightLabel.SetActive(false);
        }
    }

    public void pressFightButton()
    {
        this.gameObject.SetActive(false);
        settingUI.SetActive(false);
        usethis.GetComponent<UseThis>().isGameStart = true;
    }

    public void pressSettingButton()
    {
        StartCoroutine(WaitAndSwitchToSetting());
    }

    IEnumerator WaitAndSwitchToSetting()
    {
        settingScreen.SetActive(true);
        yield return 0.5f;
        this.gameObject.SetActive(false);
        isSettingOpen = true;
    }

    public void pressConfirmButton()
    {
        isSettingOpen = false;
        this.gameObject.SetActive(true);
        settingScreen.SetActive(false);
    }
}
