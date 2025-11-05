using UnityEngine;

public class BeaconAnchor : MonoBehaviour
{
    [Header("iBeacon identity")]
    public string uuid;
    public int major;
    public int minor;

    [Header("RF model")]
    [Tooltip("RSSI at 1ft (dBm). Calibrate if possible.")]
    public int txPowerAt1m = -59;

    [Tooltip("Indoor path-loss exponent 1.8–2.6 typical.")]
    [Range(1.5f, 3f)] public float n = 2.0f;

    public string Key => $"{uuid}_{major}_{minor}".ToUpperInvariant();
}
