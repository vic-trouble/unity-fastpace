using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    // primary animation
    private Animator animator;

    private float SPEED = 0.25f;

    private bool isRight = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per tick
    void FixedUpdate()
    {
        if (Input.GetButton("Fire1")) {
            animator.Play("Shoot");
            return;
        }

        // else move
        float move = Input.GetAxis("Horizontal") * SPEED;

        transform.position = new Vector3(transform.position.x + move, transform.position.y, transform.position.z);

        if (Mathf.Abs(move) > 0.01)
            animator.Play("Walk");
        else
            animator.Play("Idle");

        //  flip it
        if (isRight && move < 0) {
            isRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        } else if (!isRight && move > 0) {
            isRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
