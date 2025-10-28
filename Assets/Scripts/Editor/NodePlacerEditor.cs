// Assets/Scripts/Editor/NodePlacerEditor.cs
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(NavGraph))]
public class NodePlacerEditor : Editor
{
    private const float pickRadius = 0.25f;
    private Node firstSelected;
    private bool connectMode;

    // NEW: local snap control (Unity 6 removed EditorSnapSettings.enabled)
    private bool snapToGrid = true;
    private Vector2 snapSize = new Vector2(0.5f, 0.5f);

    private static GameObject nodePrefab;
    private static string prefabSearch = "Node t:prefab";

    private void OnEnable()
    {
        if (!nodePrefab)
        {
            var guid = AssetDatabase.FindAssets(prefabSearch).FirstOrDefault();
            if (!string.IsNullOrEmpty(guid))
                nodePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
        }
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        nodePrefab = (GameObject)EditorGUILayout.ObjectField("Node Prefab", nodePrefab, typeof(GameObject), false);
        connectMode = EditorGUILayout.ToggleLeft("Connect Mode (link nodes)", connectMode);

        // NEW: snap UI
        EditorGUILayout.BeginHorizontal();
        snapToGrid = EditorGUILayout.ToggleLeft("Snap", snapToGrid, GUILayout.Width(60));
        using (new EditorGUI.DisabledScope(!snapToGrid))
        {
            snapSize.x = EditorGUILayout.FloatField("X", Mathf.Max(0.01f, snapSize.x));
            snapSize.y = EditorGUILayout.FloatField("Y", Mathf.Max(0.01f, snapSize.y));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(
            "Shift+Click: place node\n" +
            "Click node: select\n" +
            "Connect Mode: click two nodes to link\n" +
            "Delete: remove selected node\n" +
            "Esc: clear selection", MessageType.Info);
    }

    private void DuringSceneGUI(SceneView sv)
    {
        var graph = (NavGraph)target;
        Event e = Event.current;

        if (firstSelected)
        {
            Handles.color = Color.yellow;
            Handles.DrawSolidDisc(firstSelected.transform.position, Vector3.forward, 0.18f);
        }

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            // FIX 1: use Vector3, not Vector2
            Vector3 world = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
            world.z = 0f;

            // FIX 2: local snap (no EditorSnapSettings.enabled)
            if (snapToGrid)
            {
                world.x = Mathf.Round(world.x / snapSize.x) * snapSize.x;
                world.y = Mathf.Round(world.y / snapSize.y) * snapSize.y;
            }

            Node hit = FindNearestNode(graph, (Vector2)world);

            if (e.shift)
            {
                PlaceNode(graph, world);
                e.Use();
            }
            else if (hit)
            {
                if (!connectMode)
                {
                    firstSelected = hit;
                }
                else
                {
                    if (!firstSelected) firstSelected = hit;
                    else if (firstSelected != hit)
                    {
                        Link(firstSelected, hit);
                        firstSelected = null;
                    }
                }
                e.Use();
            }
            else
            {
                firstSelected = null;
            }
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete && firstSelected)
        {
            DeleteNode(graph, firstSelected);
            firstSelected = null;
            e.Use();
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            firstSelected = null;
            e.Use();
        }
    }

    private Node FindNearestNode(NavGraph graph, Vector2 pos)
    {
        float best = pickRadius * pickRadius;
        Node bestNode = null;
        foreach (var n in graph.nodes)
        {
            if (!n) continue;
            float d2 = ((Vector2)n.transform.position - pos).sqrMagnitude;
            if (d2 < best) { best = d2; bestNode = n; }
        }
        return bestNode;
    }

    private void PlaceNode(NavGraph graph, Vector3 pos)
    {
        if (!nodePrefab)
        {
            Debug.LogWarning("Assign a Node Prefab in the inspector.");
            return;
        }
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(nodePrefab, graph.transform);
        Undo.RegisterCreatedObjectUndo(go, "Place Node");
        go.transform.position = pos;

        var node = go.GetComponent<Node>();
        node.id = graph.nodes.Count > 0 ? graph.nodes.Max(n => n ? n.id : -1) + 1 : 0;

        Undo.RecordObject(graph, "Add Node Ref");
        graph.nodes.Add(node);
        EditorUtility.SetDirty(graph);
    }

    private void Link(Node a, Node b)
    {
        Undo.RecordObject(a, "Link Nodes A");
        Undo.RecordObject(b, "Link Nodes B");
        if (!a.neighbors.Contains(b)) a.neighbors.Add(b);
        if (!b.neighbors.Contains(a)) b.neighbors.Add(a);
        EditorUtility.SetDirty(a);
        EditorUtility.SetDirty(b);
    }

    private void DeleteNode(NavGraph graph, Node node)
    {
        foreach (var n in graph.nodes)
        {
            if (!n) continue;
            if (n.neighbors.Contains(node))
            {
                Undo.RecordObject(n, "Unlink Node");
                n.neighbors.Remove(node);
                EditorUtility.SetDirty(n);
            }
        }

        Undo.RecordObject(graph, "Remove Node Ref");
        graph.nodes.Remove(node);
        EditorUtility.SetDirty(graph);

        Undo.DestroyObjectImmediate(node.gameObject);
    }
}
#endif
