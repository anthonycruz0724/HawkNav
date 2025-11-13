using UnityEngine;

public class MenuCameraFollow : MonoBehaviour
{
    public Transform target;   // userIcon
    public float smoothSpeed = 5f;
    public float cameraZ = -10f;

    void LateUpdate()
    {
        // If target is missing, try to find it in the 2DMap scene
        if (target == null)
        {
            var userIconObj = GameObject.FindWithTag("Player");
            if (userIconObj != null)
                target = userIconObj.transform;

            return; // don't move camera until target found
        }

        // Follow the target
        Vector3 desired = new Vector3(target.position.x, target.position.y, cameraZ);
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}
