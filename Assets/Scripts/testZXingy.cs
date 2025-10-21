using UnityEngine;
using ZXing;  // should compile

public class ZXingTest : MonoBehaviour
{
    void Start()
    {
        IBarcodeReader reader = new BarcodeReader();
        Debug.Log("ZXing loaded successfully!");
    }
}