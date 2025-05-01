using UnityEngine;
using TMPro;  // TextMeshPro 쓰는 경우

public class DebugLogger : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    private string logMessages = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logMessages += logString + "\n";
        debugText.text = logMessages;
    }
}
