using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NavGraph : MonoBehaviour
{
    public static NavGraph Instance { get; private set; }
    public List<Node> nodes = new List<Node>();
    public Dictionary<int, Node> locationMap;
    public Dictionary<string, Node> nameToNode;
    private void Awake()
    {
        RefreshNodes();
        Instance = this;
    }

    [ContextMenu("Refresh Nodes")]
    public void RefreshNodes()
    {

        nodes.Clear();
        locationMap = new Dictionary<int, Node>();
        nameToNode = new Dictionary<string, Node>();
        int i = 0;
        foreach (Transform t in transform)
        {
            var n = t.GetComponent<Node>();
            if (!n) continue;
            n.id = i++;
            nodes.Add(n);
            locationMap.Add(n.minor, n);
            nameToNode.Add(n.shortname, n);
        }
#if UNITY_EDITOR
        Debug.Log($"NavGraph: {nodes.Count} nodes.");
#endif
    }
    private void OnValidate() => RefreshNodes();
    private void OnTransformChildrenChanged() => RefreshNodes();

    public Node NearestNode(Vector2 worldPos)
    {
        Node best = null;
        float bestDist = float.PositiveInfinity;
        foreach (var n in nodes)
        {
            float d = ((Vector2)n.transform.position - worldPos).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = n; }
        }
        return best;
    }
}
