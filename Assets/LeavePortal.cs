using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavePortal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Shooter") {
            var roofs = GameObject.Find("Map/Buildings/Roof");
            roofs.GetComponent<RoofController>().Show();
        }
    }
}
