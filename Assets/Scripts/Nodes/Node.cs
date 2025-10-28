// Assets/Scripts/Runtime/Node.cs
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int id = -1;
    public List<Node> neighbors = new List<Node>();
}
