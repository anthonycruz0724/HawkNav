using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

[ExecuteAlways]
public class GeoJsonRoomsOverlay : MonoBehaviour
{
    [Header("Source")]
    public string geoJsonFileName = "Delta_F1_Floorplan.geojson"; // under Assets/StreamingAssets
    public SpriteRenderer floorSprite;

    [Header("Style")]
    [Min(0.001f)] public float outlineWidth = 0.05f;
    public int sortingOrderOffset = 5;

    [Header("Auto-fit")]
    public bool flipY = true;                     // many exports are Y-down
    public bool useContentCrop = false;           // pixel crop inside the sprite
    public Rect contentPx = new Rect(0, 0, 0, 0); // (x,y,w,h) in sprite pixels

    [Header("Corner Fit (no pixels)")]
    public bool useCornerTransforms = false;      // use two transforms instead of pixels
    public Transform cornerTopLeft;               // place on top-left inner corner of map
    public Transform cornerBottomRight;           // place on bottom-right inner corner

    [Header("Manual Alignment (applied to root)")]
    public float rotationFixDeg = 0f;             // Z rotation
    public Vector2 scaleFixXY = Vector2.one;      // non-uniform scale
    public Vector2 offsetUnits = Vector2.zero;    // local offset (units)

    [Header("Behavior")]
    public bool preserveAlignmentOnRegenerate = true; // keep root transform on regen

    Transform _root;                  // holds all LineRenderers
    Material _lineMat;

    // ---------------- Public actions ----------------

    [ContextMenu("Regenerate")]
    public void Regenerate()
    {
        if (floorSprite == null)
        {
            Debug.LogWarning("Assign 'floorSprite' first.");
            return;
        }

        EnsureRoot(); // make sure _root exists and is parented once

        // Capture the current root transform so regen won't blow it away
        Vector3 savedPos   = _root.localPosition;
        Quaternion savedRot = _root.localRotation;
        Vector3 savedScale = _root.localScale;

        // Remove only old polygon children (keep _root itself)
        ClearPolygonChildren(_root);

        // Read file
        string path = Path.Combine(Application.streamingAssetsPath, geoJsonFileName);
        if (!File.Exists(path)) { Debug.LogError($"GeoJSON not found at {path}"); return; }

        JObject parsed;
        try { parsed = JObject.Parse(File.ReadAllText(path)); }
        catch (System.Exception e) { Debug.LogError($"Invalid GeoJSON: {e.Message}"); return; }

        var features = parsed["features"] as JArray;
        if (features == null) { Debug.LogError("No 'features' array in GeoJSON."); return; }

        if (_lineMat == null) _lineMat = new Material(Shader.Find("Sprites/Default"));

        // --- PASS 1: collect rings + bounds
        var ringsList = new List<JArray>();
        float minX = float.PositiveInfinity, maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;

        foreach (var f in features)
        {
            var geom = f["geometry"]; if (geom == null) continue;
            var type = geom["type"]?.ToString();

            if (type == "Polygon")
            {
                var rings = (JArray)geom["coordinates"];
                if (rings != null) AddRings(rings, ringsList, ref minX, ref maxX, ref minY, ref maxY);
            }
            else if (type == "MultiPolygon")
            {
                var polys = (JArray)geom["coordinates"]; if (polys == null) continue;
                foreach (var poly in polys) AddRings((JArray)poly, ringsList, ref minX, ref maxX, ref minY, ref maxY);
            }
        }
        if (ringsList.Count == 0) { Debug.LogWarning("No polygon rings found."); return; }

        // --- SPRITE geometry (convert mapping target -> sprite local)
        var spr = floorSprite.sprite;
        float ppu = spr.pixelsPerUnit;
        Rect sprRectPx = spr.rect;
        Vector2 pivotPx = spr.pivot;

        // Determine target rect in sprite-local units
        bool useCorners = useCornerTransforms && cornerTopLeft && cornerBottomRight;
        Vector3 tlLocal, brLocal;

        if (useCorners)
        {
            // World -> sprite local
            tlLocal = floorSprite.transform.InverseTransformPoint(cornerTopLeft.position);
            brLocal = floorSprite.transform.InverseTransformPoint(cornerBottomRight.position);
        }
        else
        {
            Rect targetPx = sprRectPx;
            if (useContentCrop && contentPx.width > 0 && contentPx.height > 0)
            {
                targetPx = new Rect(
                    sprRectPx.x + contentPx.x,
                    sprRectPx.y + contentPx.y,
                    contentPx.width,
                    contentPx.height
                );
            }

            float leftL   = (targetPx.x - sprRectPx.x - pivotPx.x) / ppu;
            float rightL  = (targetPx.x + targetPx.width  - sprRectPx.x - pivotPx.x) / ppu;
            float bottomL = (targetPx.y - sprRectPx.y - pivotPx.y) / ppu;
            float topL    = (targetPx.y + targetPx.height - sprRectPx.y - pivotPx.y) / ppu;

            tlLocal = new Vector3(leftL,  topL,  0f);
            brLocal = new Vector3(rightL, bottomL, 0f);
        }

        int created = 0;

        // --- PASS 2: build lines (root-local so parent transforms affect them)
        foreach (var ring in ringsList)
        {
            int count = ring.Count;
            if (CoordinatesEqual(ring[0], ring[count - 1])) count--;

            var go = new GameObject("Polygon_Exterior");
            go.transform.SetParent(_root, false);

            var lr = go.AddComponent<LineRenderer>();
            lr.sharedMaterial = _lineMat;
            lr.widthMultiplier = outlineWidth;
            lr.sortingOrder = floorSprite.sortingOrder + sortingOrderOffset;
            lr.loop = true;
            lr.useWorldSpace = false;    // IMPORTANT
            lr.numCornerVertices = 2;
            lr.positionCount = count;

            for (int i = 0; i < count; i++)
            {
                float x = (float)ring[i][0];
                float y = (float)ring[i][1];

                // Normalize within Geo bounds
                float nx = Mathf.InverseLerp(minX, maxX, x);
                float ny = Mathf.InverseLerp(minY, maxY, y);
                if (flipY) ny = 1f - ny;

                // Lerp inside target rect (sprite local)
                float lx = Mathf.Lerp(tlLocal.x, brLocal.x, nx);
                float ly = Mathf.Lerp(tlLocal.y, brLocal.y, ny);
                Vector3 spriteLocal = new Vector3(lx, ly, -0.01f);

                // sprite-local -> world -> root-local
                Vector3 world = floorSprite.transform.TransformPoint(spriteLocal);
                Vector3 rootLocal = _root.InverseTransformPoint(world);
                lr.SetPosition(i, rootLocal);
            }

            created++;
        }

        // Restore or apply alignment
        if (preserveAlignmentOnRegenerate)
        {
            _root.localPosition = savedPos;
            _root.localRotation = savedRot;
            _root.localScale    = savedScale;
        }
        else
        {
            ApplyAlignmentOnly(); // use sliders if you want a fresh apply each time
        }

        Debug.Log($"GeoJSON overlay generated. Polygons created: {created}");
    }

    [ContextMenu("Apply Alignment Only")]
    public void ApplyAlignmentOnly()
    {
        if (_root == null) return;
        _root.localRotation = Quaternion.Euler(0f, 0f, rotationFixDeg);
        _root.localScale    = new Vector3(
            Mathf.Max(0.0001f, scaleFixXY.x),
            Mathf.Max(0.0001f, scaleFixXY.y),
            1f
        );
        _root.localPosition = new Vector3(offsetUnits.x, offsetUnits.y, 0f);
    }

    void OnValidate()
    {
        if (!isActiveAndEnabled) return;
        if (_root != null) ApplyAlignmentOnly(); // live slider updates
    }

    // ---------------- internals ----------------

    void EnsureRoot()
    {
        if (_root == null)
        {
            // Prefer an existing child named GeoJSON_Rooms under the floor sprite
            Transform parent = floorSprite.transform;
            var found = parent.Find("GeoJSON_Rooms");
            _root = found ? found : new GameObject("GeoJSON_Rooms").transform;
            _root.SetParent(parent, false);
        }

        if (_lineMat == null) _lineMat = new Material(Shader.Find("Sprites/Default"));
    }

    static void ClearPolygonChildren(Transform t)
    {
        // delete only polygon children we previously created
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            var child = t.GetChild(i);
            if (child.GetComponent<LineRenderer>() == null) continue; // keep non-polygon helpers
#if UNITY_EDITOR
            if (!Application.isPlaying) Object.DestroyImmediate(child.gameObject);
            else Object.Destroy(child.gameObject);
#else
            Object.Destroy(child.gameObject);
#endif
        }
    }

    static void AddRings(JArray rings, List<JArray> outList,
        ref float minX, ref float maxX, ref float minY, ref float maxY)
    {
        foreach (var r in rings)
        {
            var ring = (JArray)r;
            if (ring == null || ring.Count < 4) continue;

            for (int i = 0; i < ring.Count; i++)
            {
                float x = (float)ring[i][0];
                float y = (float)ring[i][1];
                if (x < minX) minX = x; if (x > maxX) maxX = x;
                if (y < minY) minY = y; if (y > maxY) maxY = y;
            }
            outList.Add(ring);
        }
    }

    static bool CoordinatesEqual(JToken a, JToken b)
    {
        const float eps = 1e-5f;
        float ax = (float)a[0], ay = (float)a[1];
        float bx = (float)b[0], by = (float)b[1];
        return Mathf.Abs(ax - bx) < eps && Mathf.Abs(ay - by) < eps;
    }

#if UNITY_EDITOR
    void OnDisable()
    {
        if (!Application.isPlaying && _lineMat != null)
        {
            DestroyImmediate(_lineMat);
            _lineMat = null;
        }
    }
#endif
}
