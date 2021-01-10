using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSortingLayer : MonoBehaviour
{
    public int sortingOrder;
    public string sortingLayerName;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = sortingLayerName;
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = sortingOrder;
    }
}
