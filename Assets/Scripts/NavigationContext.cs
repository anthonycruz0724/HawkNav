using UnityEngine;

//Stores global navigation data that persists across scenes

public class NavigationContext
{
    public static string StartLocation { get; private set; } = "";
    public static string EndLocation { get; private set; } = "";

    public static void SetLocations(string start, string end)
    {
        StartLocation = start;
        EndLocation = end;
        Debug.Log($"[NavigationContext] StartLocation set to: {StartLocation}, EndLocation set to: {EndLocation}");
    }

    public static void Clear()
    {
        StartLocation = "";
        EndLocation = "";
        Debug.Log("[NavigationContext] Cleared StartLocation and EndLocation");
    }
}
