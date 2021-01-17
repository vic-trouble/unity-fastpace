using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private enum State
    {
        Close,
        Opening,
        Open
    }

    private State state = State.Close;

    private bool open = false;

    private float disableColliderTime = 0;

    void FixedUpdate()
    {
        if (disableColliderTime != 0 && Time.fixedTime > disableColliderTime) {
            GetComponent<Collider2D>().enabled = false;
            disableColliderTime = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<UnitController>()) {
            if (!open) {
                open = false;
                GetComponent<Animator>().SetBool("Open", true);
                disableColliderTime = Time.fixedTime + 0.2f;
            }
        }
    }
}
