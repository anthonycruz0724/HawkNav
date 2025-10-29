using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NavGraph : MonoBehaviour
{
    public List<Node> nodes = new List<Node>();

    [ContextMenu("Refresh Nodes")]
    public void RefreshNodes()
    {
        nodes.Clear();
        int i = 0;
        foreach (Transform t in transform)
        {
            var n = t.GetComponent<Node>();
            if (!n) continue;
            n.id = i++;
            nodes.Add(n);
        }
#if UNITY_EDITOR
        Debug.Log($"NavGraph: {nodes.Count} nodes.");
#endif
    }

    private void OnValidate() => RefreshNodes();
    private void OnTransformChildrenChanged() => RefreshNodes();

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
        foreach (var n in nodes)
        {
            if (!n) continue;
            foreach (var nb in n.neighbors)
            {
                if (nb) Gizmos.DrawLine(n.transform.position, nb.transform.position);
            }
        }
    }
}
