using System;
using System.Collections.Generic;
using UnityEngine;

public class PersonNavigator : MonoBehaviour
{
    [Header("Scene refs")]
    public NavGraph graph;          // assign NavGraph_Floor1
    public Transform person;        // the marker for the user (sprite, icon, etc.)
    public Camera cam;              // usually Main Camera

    [Header("Connection settings")]
    public float connectRadius = 3f;      // how far to search for nodes to link
    public LayerMask obstacleMask;        // set to your Obstacles (if none, leave 0)
    public int maxLinks = 2;              // connect to the 1–2 closest nodes

    [Header("Click picking")]
    public LayerMask navLayer;            // set to Nav

    [Header("Path line")]
    public Color lineColor = Color.yellow;
    public float lineWidth = 0.06f;
    public string sortingLayer = "Default";
    public int sortingOrder = 60;

    private LineRenderer line;
    private Node targetNode;

    void Awake()
    {
        if (!cam) cam = Camera.main;

        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = line.endWidth = lineWidth;
        line.startColor = line.endColor = lineColor;
        line.sortingLayerName = sortingLayer;
        line.sortingOrder = sortingOrder;
        line.positionCount = 0;
        line.useWorldSpace = true;
        line.numCapVertices = 2;
    }

    void Update()
    {
        // Left-click: pick a target node
        if (Input.GetMouseButtonDown(0))
        {
            var picked = PickNodeUnderMouse();
            if (picked)
            {
                targetNode = picked;
                Debug.Log($"Target node set: {targetNode.id}");
                ComputeAndDrawPath();
            }
        }
    }

    Node PickNodeUnderMouse()
    {
        Vector3 m = cam.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(m, navLayer);
        if (!hit) return null;
        return hit.GetComponent<Node>();
    }

    void ComputeAndDrawPath()
    {
        if (!graph || person == null || targetNode == null)
        {
            Debug.LogWarning("Missing graph, person, or target.");
            return;
        }

        // 1) Create a TEMP start node at the person's position
        var tempGo = new GameObject("TempStartNode");
        tempGo.transform.SetParent(graph.transform, false);
        tempGo.transform.position = new Vector3(person.position.x, person.position.y, 0f);
        var tempNode = tempGo.AddComponent<Node>();
        tempNode.id = -1; // special, not in graph.nodes

        // 2) Connect temp node to nearest graph nodes (1–2 links)
        var nearest = FindNearestVisibleNodes(tempNode.transform.position, connectRadius, maxLinks);
        foreach (var n in nearest)
        {
            // Link both ways
            if (!tempNode.neighbors.Contains(n)) tempNode.neighbors.Add(n);
            if (!n.neighbors.Contains(tempNode)) n.neighbors.Add(tempNode);
        }

        // 3) Run A* from temp node to target
        var path = AStar.FindPath(tempNode, targetNode);

        // 4) Draw the path (and compute length)
        if (path == null || path.Count == 0)
        {
            line.positionCount = 0;
            Debug.LogWarning("No path found.");
        }
        else
        {
            line.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
                line.SetPosition(i, path[i]);

            float dist = 0f;
            for (int i = 1; i < path.Count; i++)
                dist += Vector3.Distance(path[i - 1], path[i]);
            Debug.Log($"Path nodes: {path.Count}, length ~ {dist:0.00}");
        }

        // 5) Clean up: unlink temp node from real nodes and destroy it
        foreach (var n in nearest)
            if (n) n.neighbors.Remove(tempNode);
        DestroyImmediate(tempGo);
    }

    List<Node> FindNearestVisibleNodes(Vector3 pos, float radius, int maxCount)
    {
        var list = new List<(Node node, float d2)>();
        foreach (var n in graph.nodes)
        {
            if (!n) continue;
            float d2 = (n.transform.position - pos).sqrMagnitude;
            if (d2 > radius * radius) continue;

            // Optional line-of-sight check
            if (obstacleMask.value != 0)
            {
                var hit = Physics2D.Linecast(pos, n.transform.position, obstacleMask);
                if (hit.collider) continue;
            }
            list.Add((n, d2));
        }

        list.Sort((a, b) => a.d2.CompareTo(b.d2));

        var result = new List<Node>();
        for (int i = 0; i < Mathf.Min(maxCount, list.Count); i++)
            result.Add(list[i].node);
        return result;
    }
}
