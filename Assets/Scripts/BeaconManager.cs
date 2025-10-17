using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class BeaconManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void startBeaconStream();

    [Header("UI Output")]
    public Text outputText;            // assign in Inspector

    void Start()
    {
#if UNITY_IOS && !UNITY_EDITOR
        startBeaconStream();
#else
        Debug.Log("Running in Editor – plugin disabled");
#endif
        if (outputText != null)
            outputText.text = "Starting beacon scan…";
    }

    public void OnBeaconUpdate(string message)
    {
        Debug.Log("Beacon JSON: " + message);
        if (outputText != null)
            outputText.text = message;
    }
}

