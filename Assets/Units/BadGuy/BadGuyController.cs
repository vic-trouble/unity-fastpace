using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadGuyController : UnitController
{
    // Start is called before the first frame update
    void Start()
    {
        Init(GetComponent<Animator>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var renderer = GetComponent<SpriteRenderer>();

        // am I still alive?
        if (health <= 0) {
            renderer.sortingLayerName = "DeadBodies";
            return;
        }

        // rotate to direction
        Vector2 aimPosition = GameObject.Find("Shooter").transform.position;
        float direction = -Vector2.SignedAngle(aimPosition - (Vector2)transform.position, Vector2.right);
        if (direction < 0)
            direction += 360;

        // flip if necessary
        GetComponent<SpriteRenderer>().flipX = GetFlipX(direction) < 0;

        // animate
        string animation = "Idle";
        PlayAnimatinon(GetAnimPrefix(direction) + animation);

        // proper Y-sorting
        renderer.sortingOrder = (int)(-transform.position.y * 10);
    }
}
