using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public int NUM_GRENADES = 3;

    void OnTriggerEnter2D(Collider2D collider)
    {
        var shooter = collider.gameObject.GetComponent<ShooterController>();
        if (shooter) {
            shooter.PickUpGrenades(NUM_GRENADES);
            Destroy(transform.parent.gameObject);
        }
    }
}
