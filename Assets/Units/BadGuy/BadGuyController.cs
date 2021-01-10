using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BadGuyState
{
    Idle,
    Aggravated,
    Dead
}

public class BadGuyController : UnitController
{
    private BadGuyState state = BadGuyState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        Init(GetComponent<Animator>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state == BadGuyState.Dead)
            return;

        // rotate to direction
        Vector2 aimPosition = GameObject.Find("Shooter").transform.position;
        float direction = -Vector2.SignedAngle(aimPosition - (Vector2)transform.position, Vector2.right);
        if (direction < 0)
            direction += 360;

        // flip if necessary
        GetComponent<SpriteRenderer>().flipX = GetFlipX(direction) < 0;

        // animate
        string animation = "Idle";
        if (state == BadGuyState.Aggravated) {
            Vector2 delta = aimPosition - (Vector2)transform.position;
            float moveX = Mathf.Sign(delta.x) * Time.fixedDeltaTime;
            float moveY = Mathf.Sign(delta.y) * Time.fixedDeltaTime;
            Move(moveX, moveY);
            animation = "Walk";
        }
        PlayAnimatinon(GetAnimPrefix(direction) + animation);

        // proper Y-sorting
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sortingOrder = (int)(-transform.position.y * 10);
    }

    protected override void OnHit()
    {
        if (state == BadGuyState.Idle)
            state = BadGuyState.Aggravated;
    }

    protected override void OnDie()
    {
        state = BadGuyState.Dead;
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sortingLayerName = "DeadBodies";
    }
}
