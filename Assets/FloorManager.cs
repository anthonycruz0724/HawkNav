using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [Header("Floor Roots")]
    public GameObject floor1;
    public GameObject floor2;

    [Header("Camera (optional)")]
    public Camera mainCam;
    public Transform floor1CamTarget;
    public Transform floor2CamTarget;
    public float camLerpSpeed = 4f;

    int currentFloor = 1;
    Transform target;

    void Start()
    {
        ShowFloor(1); // start with floor 1 active
    }

    public void ShowFloor(int floor)
    {
        currentFloor = Mathf.Clamp(floor, 1, 2);
        bool onF1 = currentFloor == 1;

        floor1.SetActive(onF1);
        floor2.SetActive(!onF1);

        if (mainCam != null)
            target = onF1 ? floor1CamTarget : floor2CamTarget;
    }

    void LateUpdate()
    {
        if (mainCam != null && target != null)
        {
            var p = mainCam.transform.position;
            var t = new Vector3(target.position.x, target.position.y, p.z);
            mainCam.transform.position = Vector3.Lerp(p, t, Time.deltaTime * camLerpSpeed);
        }
    }
}

