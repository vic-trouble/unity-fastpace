using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // remove transparency
        Show();
    }

    public void Show()
    {
        GetComponent<Tilemap>().color = Color.white;
    }

    public void Hide()
    {
        GetComponent<Tilemap>().color = new Color(0, 0, 0, 0);
    }
}
