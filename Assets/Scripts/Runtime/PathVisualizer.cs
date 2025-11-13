using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
    public NavGraph graph;
    public Transform userIcon; // your player icon transform
    public Node destination;

    private LineRenderer line;
    private List<Node> currentPath;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = line.endColor = Color.cyan;
    }

    void Update()
    {
        destination = NavigationContext.EndLocation;
        if (!graph || !userIcon || !destination) return;
        Node start = NavigationContext.StartLocation;
        if (start == null) return;
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
