using UnityEngine;
using System.Linq;
using System;
public class BeaconMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

       var sorted = BeaconManager.Instance.currentBeacons
                        .OrderBy(b => ProximityRank(b.proximity)).ThenByDescending(b => b.rssi).ToList();

        if (sorted != null)
        {
            var closestBeacon = sorted.FirstOrDefault();
            NavGraph graph = NavGraph.Instance;
            if (closestBeacon != null && graph.locationMap.ContainsKey(closestBeacon.minor))
            {
                transform.position = graph.locationMap[closestBeacon.minor].transform.position;
            }
            else
            {
                Debug.LogWarning($"No location mapped for beacon minor {closestBeacon.minor}");
            }

        }


    }

    // Update is called once per frame
    void Update()
    {
        if(BeaconManager.Instance.currentBeacons != null || BeaconManager.Instance.currentBeacons.Length != 0)
        {
                    var sorted = BeaconManager.Instance.currentBeacons
                 .OrderBy(b => ProximityRank(b.proximity)).ThenByDescending(b => b.rssi).ToList();

            if (sorted != null)
            {
                var closestBeacon = sorted.FirstOrDefault();
                NavGraph graph = NavGraph.Instance;
                if (closestBeacon != null && graph.locationMap.ContainsKey(closestBeacon.minor))
                {
                    transform.position = graph.locationMap[closestBeacon.minor].transform.position;
                }
                else
                {
                    Debug.LogWarning($"No location mapped for beacon minor {closestBeacon.minor}");
                }

            }
        }

    }
        private int ProximityRank(string proximity)
    {
        // Lower rank = closer
        return proximity switch
        {
            "immediate" => 0,
            "near" => 1,
            "far" => 2,
            _ => 3
        };
    }
}
