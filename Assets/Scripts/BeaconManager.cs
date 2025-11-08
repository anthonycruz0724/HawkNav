using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BeaconData
{
    public string uuid;
    public int major;
    public int minor;
    public int rssi;
    public string proximity;
}

[System.Serializable]
public class BeaconList
{
    public BeaconData[] items;
}

public class BeaconManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void startBeaconStream();

    public static BeaconManager Instance { get; private set; }

    [Header("UI Output")]
    public Text outputText;

    public BeaconData[] currentBeacons;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        try
        {
            string wrapped = "{\"items\":" + message + "}";
            BeaconList list = JsonUtility.FromJson<BeaconList>(wrapped);

            if (list != null && list.items != null)
            {
                currentBeacons = list.items;

                if (outputText != null)
                {
                    outputText.text = $"Detected {currentBeacons.Length} beacon(s):\n";
                    foreach (var b in currentBeacons)
                        outputText.text += $"{b.uuid} RSSI={b.rssi} ({b.proximity})\n";
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("JSON Parse Error: " + ex.Message);
            if (outputText != null)
                outputText.text = "Error parsing beacon data";
        }
    }
}

