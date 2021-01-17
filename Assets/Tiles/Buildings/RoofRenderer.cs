using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // remove transparency
        GetComponent<Tilemap>().color = Color.white;
    }
}
