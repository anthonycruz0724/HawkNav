using System;
using UnityEngine;
using Vuforia;

public class SimpleBarcodeScanner : MonoBehaviour
{

    public TMPro.TextMeshProUGUI barcodeAsText;
    BarcodeBehaviour mBarcodeBehaviour;
    void Start()
    {
        mBarcodeBehaviour = GetComponent<BarcodeBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mBarcodeBehaviour != null && mBarcodeBehaviour.InstanceData != null)
        {
            barcodeAsText.text = mBarcodeBehaviour.InstanceData.Text;
            Debug.Log($"[SimpleBarcodeScanner] Detected barcode: {barcodeAsText.text}");
            
            QRCodeData qrCodeData = JsonUtility.FromJson<QRCodeData>(barcodeAsText.text);
            Debug.Log($"[SimpleBarcodeScanner] Parsed QRCodeData: ID={qrCodeData.id}, Name={qrCodeData.name}, Floor={qrCodeData.floor}, X={qrCodeData.x}, Y={qrCodeData.y}");
            NavigationContext.SetLocations(qrCodeData.id, "DestinationRoom"); // Set destination as needed
            SceneLoader.Instance.LoadScene("MainMenu");
        }
        else
        {
            barcodeAsText.text = "";
        }
    }


}

