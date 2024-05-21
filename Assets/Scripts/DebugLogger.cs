using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DebugLogger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textbox;

    public static DebugLogger Instance { get; private set; }
    private void Awake()
    {
        if(Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    private void LogInternal(string message, Action<string> loggingMethod)
    {
        loggingMethod(message);
        textbox.text = message;
    }
    public void Log(string message) => LogInternal(message, Debug.Log);
    public void Log(object message) => LogInternal(message.ToString(), Debug.Log);
    public void LogError(string message) => LogInternal(message, Debug.LogError);
    public void LogError(object message) => LogInternal(message.ToString(), Debug.LogError);
}
