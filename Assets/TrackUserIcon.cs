using UnityEngine;

public class TrackUserIcon : MonoBehaviour
{

    public Transform user;

    void LateUpdate()
    {
        if (user)
            transform.position = new Vector3(user.position.x, user.position.y, -10f);
    }
}


