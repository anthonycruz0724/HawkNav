// PlayerManualController2D.cs
using UnityEngine;

public class PlayerManualController2D : MonoBehaviour
{
    public float speed = 3.5f;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 d = new Vector3(h, v, 0f).normalized;
        transform.position += d * speed * Time.deltaTime;
    }
}
