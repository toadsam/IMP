using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SHBUIController : MonoBehaviour
{
    public GameObject startScreen;       // The initial UI shown when the game starts
    public GameObject scanningScreen;    // UI shown during AR scanning
    public GameObject settingUI;         // Reference to setting UI toggle (used in fight)
    public GameObject settingScreen;     // The full settings screen UI
    public GameObject fightLabel;        // Label that explains the "Fight" action
    public GameObject fightButton;       // Button that starts the battle/game
    public TextMeshProUGUI scanButtonText; // Text label of the scan toggle button
    public GameObject usethis;           // Reference to the UseThis script to trigger game start

    private bool isScanning = false;     // Whether scanning is currently active
    private bool isSettingOpen = false;  // Whether the settings screen is currently open

    void Start()
    {
        // Disable UI elements at start
        scanningScreen.SetActive(false);
        fightLabel.SetActive(false);
        fightButton.SetActive(false);
        settingScreen.SetActive(false);
    }

    public void SwitchScreens()
    {
        // Transition from the start screen to the scanning screen
        if (startScreen != null && scanningScreen != null)
        {
            startScreen.SetActive(false);
            scanningScreen.SetActive(true);
        }
    }

    public void changeScanButtonText()
    {
        // Toggle scan state and update UI accordingly
        if (isScanning)
        {
            scanButtonText.text = "Restart Scan";
            isScanning = false;

            // Enable fight option when scan is done
            fightButton.SetActive(true);
            fightLabel.SetActive(true);
        }
        else
        {
            scanButtonText.text = "Stop Scan";
            isScanning = true;

            // Hide fight UI while scanning
            fightButton.SetActive(false);
            fightLabel.SetActive(false);
        }
    }

    public void pressFightButton()
    {
        // Begin the game: disable UI, hide setting UI, and notify game start via UseThis
        this.gameObject.SetActive(false);
        settingUI.SetActive(false);
        usethis.GetComponent<UseThis>().isGameStart = true;
    }

    public void pressSettingButton()
    {
        // Coroutine is used to delay the deactivation to prevent UI flickering
        StartCoroutine(WaitAndSwitchToSetting());
    }

    IEnumerator WaitAndSwitchToSetting()
    {
        // Show setting screen first
        settingScreen.SetActive(true);

        // Small delay to allow visual update before hiding this UI
        yield return new WaitForSeconds(0.5f);

        // Then hide this UI
        this.gameObject.SetActive(false);
        isSettingOpen = true;
    }

    public void pressConfirmButton()
    {
        // When setting is done, go back to the main scanning UI
        isSettingOpen = false;
        this.gameObject.SetActive(true);
        settingScreen.SetActive(false);
    }
}
