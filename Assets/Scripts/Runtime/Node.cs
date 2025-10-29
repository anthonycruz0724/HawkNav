using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int id = -1;
    public List<Node> neighbors = new List<Node>(); // fill by hand or auto (next step)
}
