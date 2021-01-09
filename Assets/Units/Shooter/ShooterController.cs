using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : UnitController
{
    private float nextShotTime = 0;
    public float SHOT_SPEED = 0.25f;

    // primary animation
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per tick
    void FixedUpdate()
    {
        // rotate to direction
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 aimPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        float direction = -Vector2.SignedAngle(aimPosition - (Vector2)transform.position, Vector2.right);
        if (direction < 0)
            direction += 360;

        string animation = "Idle";
        bool forceAnimation = false;

        // move
        float moveX = Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
        float moveY = Input.GetAxis("Vertical") * Time.fixedDeltaTime;
        Move(moveX, moveY);

        if (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveY) > 0)
            animation = "Walk";

        // shooting
        if (Input.GetButton("Fire1") && Time.fixedTime >= nextShotTime) {
            animation = "Shoot";
            forceAnimation = true;
            Shoot(aimPosition);
            nextShotTime = Time.fixedTime + SHOT_SPEED;
        }
        else if (Time.fixedTime < nextShotTime) {
            animation = "Shoot";
            return; // NB!
        }

        // flip if necessary
        transform.localScale = new Vector3(GetFlipX(direction), 1, 1);

        // animate
        PlayAnimatinon(animator, GetAnimPrefix(direction) + animation, forceAnimation);
    }
}
