using UnityEngine;

public class MapManager : MonoBehaviour
{
    void Start()
    {
        string start = NavigationContext.StartLocation;
        string end = NavigationContext.EndLocation;

        Debug.Log($"[MapManager] Loaded navigation: {start} → {end}");

        // TODO: pass this data into your pathfinding / user marker placement system
    }
}

