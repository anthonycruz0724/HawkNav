using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
    public NavGraph graph;
    public Transform userIcon; // your player icon transform
    public Node destination;

    public Node start;
    private LineRenderer line;
    private List<Node> currentPath;

    public TMP_Text finished;

    private Dictionary<int, Node> locationMapLocal;
    void Awake()
    {
        locationMapLocal = new Dictionary<int, Node>();
        foreach (Node n in graph.nodes)
        {
            Debug.Log(n.minor);
            locationMapLocal.Add(n.minor, n);
        }
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = 1.05f;
        line.endWidth = 1.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = line.endColor = Color.cyan;
    }

    void Start()
    {
                locationMapLocal = new Dictionary<int, Node>();
        foreach (Node n in graph.nodes)
        {
            locationMapLocal.Add(n.minor, n);
        }
        if (finished == null)
            finished = GameObject.Find("FinishedText").GetComponent<TMP_Text>();
    }

    void Update()
    {
                locationMapLocal = new Dictionary<int, Node>();
        foreach (Node n in graph.nodes)
        {
            locationMapLocal.Add(n.minor, n);
        }
        try
        {
            destination = locationMapLocal[NavigationContext.EndLocation.minor];
        }
        catch (Exception e)
        {
            Debug.Log("Dictionary doesn't contain key");
        }
        if (!graph || !userIcon || !destination) return;
        start = locationMapLocal[NavigationContext.StartLocation.minor];
        Debug.Log("Second Line check");
        if (start == null) return;
        Debug.Log("Check start status");
        currentPath = Pathfinder.FindPath(start, destination);
        Debug.Log("Navigation started");
        if (currentPath != null && currentPath.Count > 1)
        {
            var sorted = BeaconManager.Instance.currentBeacons
                .OrderBy(b => ProximityRank(b.proximity)).ThenByDescending(b => b.rssi).ToList();

            var closestBeacon = sorted.FirstOrDefault();
            var locationDictionary = NavGraph.Instance.locationMap;

            if (closestBeacon.minor != 0)
            {
                string locationName = locationDictionary[closestBeacon.minor].shortname;
                Debug.Log($"Closest Beacon: {locationName}");
                NavigationContext.StartLocation = locationDictionary[closestBeacon.minor];
            }
            else
            {
                Debug.Log($"NO BEACONS NEAR");
            }

            line.positionCount = currentPath.Count;
            for (int i = 0; i < currentPath.Count; i++)
                line.SetPosition(i, currentPath[i].transform.position);
        }
        else if (currentPath != null && currentPath.Count == 1)
        {
            line.positionCount = 0;

            if (finished != null)
            {
                finished.text = "You have arrived!";
                finished.gameObject.SetActive(true);
            }
        }
        else
        {
            line.positionCount = 0;
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
