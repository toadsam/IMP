using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SHBUIController : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject scanningScreen;
    public GameObject fightLabel;
    public GameObject fightButton;
    public TextMeshProUGUI scanButtonText;
    public GameObject usethis;
    private bool isScanning = false;

    void Start()
    {
        scanningScreen.SetActive(false);
        fightLabel.SetActive(false);
        fightButton.SetActive(false);
    }
    void Update()
    {
        // 화면을 터치하거나 클릭한 경우
        if (Input.GetMouseButtonDown(0))  // 왼쪽 마우스 버튼 또는 화면 터치
        {
            SwitchScreens();
        }
    }

    void SwitchScreens()
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

    public void pressFightButton(){
        this.gameObject.SetActive(false);
        usethis.GetComponent<UseThis>().isGameStart = true;
    }
}
