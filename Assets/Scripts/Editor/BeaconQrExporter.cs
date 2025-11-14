#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public static class BeaconQrExporter
{
    [MenuItem("HawkNav/Export Beacon QR JSON")]
    public static void Export()
    {
        Node[] nodes = Object.FindObjectsOfType<Node>();
        BeaconQrDatabase db = new BeaconQrDatabase();

        foreach (var node in nodes)
        {
            db.beacons.Add(new BeaconQrEntry
            {
                shortname = node.shortname,
                minor     = node.minor
            });
        }

        string folder = "Assets/Resources";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, "beacon_qr_db.json");
        string json = JsonUtility.ToJson(db, true);

        File.WriteAllText(path, json);
        AssetDatabase.Refresh();

        Debug.Log($"Exported {db.beacons.Count} beacon nodes to: {path}");
    }
}
#endif
