using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadGuyController : UnitController
{
    // primary animation
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // rotate to direction
        Vector2 aimPosition = GameObject.Find("Shooter").transform.position;
        float direction = -Vector2.SignedAngle(aimPosition - (Vector2)transform.position, Vector2.right);
        if (direction < 0)
            direction += 360;

        // flip if necessary
        transform.localScale = new Vector3(GetFlipX(direction), 1, 1);

        // animate
        string animation = "Idle";
        PlayAnimatinon(animator, GetAnimPrefix(direction) + animation);
    }
}
