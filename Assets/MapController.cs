using System;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public RectTransform mapRoot;     // MapRoot (RectTransform)
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f, maxZoom = 3f;

    float currentZoom = 1f;
    bool isPanning = false;
    Vector2 lastLocal;                // last pointer pos in Canvas space
    Canvas canvas;
    Camera uiCam;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        // For Screen Space - Camera we must pass the camera; for Overlay it should be null.
        uiCam = (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera) ? canvas.worldCamera : null;
    }

    void Update()
    {
        // --- Zoom (wheel) ---
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.001f)
        {
            currentZoom = Mathf.Clamp(currentZoom + scroll * zoomSpeed, minZoom, maxZoom);
            mapRoot.localScale = Vector3.one * currentZoom;
        }

        // --- Begin pan on right mouse down ---
        if (Input.GetMouseButtonDown(1))
        {
            isPanning = true;
            lastLocal = ScreenToCanvasLocal(Input.mousePosition);
        }

        // --- Pan while held ---
        if (isPanning && Input.GetMouseButton(1))
        {
            Vector2 curLocal = ScreenToCanvasLocal(Input.mousePosition);
            Vector2 delta = curLocal - lastLocal;     // delta in Canvas units
            mapRoot.anchoredPosition += delta;        // move the map
            lastLocal = curLocal;
        }

        // --- End pan ---
        if (Input.GetMouseButtonUp(1))
            isPanning = false;
    }

    // Convert screen point → parent RectTransform local space (Canvas units)
    Vector2 ScreenToCanvasLocal(Vector3 screenPos)
    {
        var parentRect = mapRoot.parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, uiCam, out var local);
        return local;
    }
}
