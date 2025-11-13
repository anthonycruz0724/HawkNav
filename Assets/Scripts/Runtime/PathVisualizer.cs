using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
    public NavGraph graph;
    public Transform userIcon; // your player icon transform
    public Node destination;

    private LineRenderer line;
    private List<Node> currentPath;

    private Dictionary<int, Node> locationMapLocal;
    void Awake()
    {
        foreach(Node n in graph.nodes){
            locationMapLocal.Add(n.minor, n);
        }
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = line.endColor = Color.cyan;
    }

    void Update()
    {

        try
        {
            destination = locationMapLocal[NavigationContext.EndLocation.minor];
        }
        catch(Exception e)
        {
            Debug.Log("Dictionary doesn't contain key");
        }
        if (!graph || !userIcon || !destination) return;
        Node start = locationMapLocal[NavigationContext.StartLocation.minor];
        Debug.Log("Second Line check");
        if (start == null) return;
        Debug.Log("Check start status");
        currentPath = Pathfinder.FindPath(start, destination);
        Debug.Log("Navigation started");
        if (currentPath != null && currentPath.Count > 1)
        {
            line.positionCount = currentPath.Count;
            for (int i = 0; i < currentPath.Count; i++)
                line.SetPosition(i, currentPath[i].transform.position);
        }
        else
        {
            line.positionCount = 0;
        }
    }
}
