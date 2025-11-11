using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    class Rec
    {
        public Node node;
        public Rec parent;
        public float g, f;
    }

    static float H(Node a, Node b) =>
        Vector2.Distance(a.transform.position, b.transform.position);

    public static List<Node> FindPath(Node start, Node goal)
    {
        if (!start || !goal) return null;

        var open = new List<Rec>();
        var all = new Dictionary<Node, Rec>();
        var closed = new HashSet<Node>();

        Rec GetRec(Node n)
        {
            if (!all.TryGetValue(n, out var r))
            {
                r = new Rec { node = n, g = float.PositiveInfinity, f = float.PositiveInfinity };
                all[n] = r;
            }
            return r;
        }

        var s = GetRec(start);
        s.g = 0f; s.f = H(start, goal);
        open.Add(s);

        while (open.Count > 0)
        {
            // pop lowest f
            int best = 0;
            for (int i = 1; i < open.Count; i++)
                if (open[i].f < open[best].f) best = i;

            var cur = open[best];
            open.RemoveAt(best);

            if (cur.node == goal) return Reconstruct(cur);

            closed.Add(cur.node);

            foreach (var nb in cur.node.neighbors)
            {
                if (!nb || closed.Contains(nb)) continue;

                float tentativeG = cur.g + Vector2.Distance(cur.node.transform.position, nb.transform.position);
                var r = GetRec(nb);

                if (tentativeG < r.g)
                {
                    r.parent = cur;
                    r.g = tentativeG;
                    r.f = tentativeG + H(nb, goal);
                    if (!open.Contains(r)) open.Add(r);
                }
            }
        }
        return null;
    }

    static List<Node> Reconstruct(Rec end)
    {
        var path = new List<Node>();
        for (var r = end; r != null; r = r.parent) path.Add(r.node);
        path.Reverse();
        return path;
    }
}
