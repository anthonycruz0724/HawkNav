using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DebuggingScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(Camera.main.name);
        // var systems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        // Debug.Log("EventSystems: " + systems.Length);

        // if (Input.touchCount > 0)
        // {
        //     var touch = Input.GetTouch(0);
        //     PointerEventData ped = new PointerEventData(EventSystem.current);
        //     ped.position = touch.position;

        //     List<RaycastResult> results = new List<RaycastResult>();
        //     EventSystem.current.RaycastAll(ped, results);

        //     foreach (var r in results)
        //     {
        //         Debug.Log("Touched: " + r.gameObject.name);
        //     }
        // }

        // Debug.Log(NavigationContext.EndLocation.shortname);
        // Debug.Log(NavigationContext.StartLocation.shortname);
    }
}
