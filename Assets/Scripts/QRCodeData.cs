using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class QRCodeData
{
    public int id;
    public string name;
    public bool isLocation;
    public float x;
    public float y;

    public List<int> connections;
        
}
