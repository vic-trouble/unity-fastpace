using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamitePackController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealDamage()
    {
        var effectsController = FindObjectOfType<EffectsController>();
        effectsController.SpawnExplosion(transform.position);
        Destroy(gameObject);
    }
}
