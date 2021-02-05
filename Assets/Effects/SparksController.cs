using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparksController : MonoBehaviour
{
    public float SPEED = 1;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += (transform.parent.position - transform.position).normalized * Time.fixedDeltaTime * SPEED;
        transform.localScale = new Vector3(Random.Range(0.5f, 1.5f), 1);
    }
}
