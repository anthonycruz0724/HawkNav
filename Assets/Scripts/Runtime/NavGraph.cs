// Assets/Scripts/Runtime/NavGraph.cs
using System.Collections.Generic;
using UnityEngine;

public class NavGraph : MonoBehaviour
{
    public List<Node> nodes = new List<Node>();  // populated by editor tool
    public Color gizmoNodeColor = new Color(0f, 1f, 1f, 0.9f);
    public Color gizmoEdgeColor = new Color(1f, 0.2f, 1f, 0.9f);
    public float gizmoNodeRadius = 0.12f;

    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        // edges
        Gizmos.color = gizmoEdgeColor;
        foreach (var n in nodes)
        {
            if (!n) continue;
            foreach (var nb in n.neighbors)
            {
                if (nb)
                    Gizmos.DrawLine(n.transform.position, nb.transform.position);
            }
        }

        // nodes
        Gizmos.color = gizmoNodeColor;
        foreach (var n in nodes)
        {
            if (!n) continue;
            Gizmos.DrawSphere(n.transform.position, gizmoNodeRadius);
        }
    }
}
