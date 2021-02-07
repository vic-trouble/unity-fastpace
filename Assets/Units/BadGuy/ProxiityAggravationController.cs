using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxiityAggravationController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        var shooter = collider.gameObject.GetComponent<ShooterController>();
        if (shooter) {
            var badGuy = transform.parent.gameObject.GetComponent<BadGuyController>();
            badGuy.Aggravate(shooter, true);
        }
    }
}
