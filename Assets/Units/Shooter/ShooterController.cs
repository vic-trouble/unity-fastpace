using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    // primary animation
    private Animator animator;

    // movement speed
    public float SPEED = 0.25f;

    // aim cursor
    public Texture2D aimCursor;

    //private bool isRight = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Cursor.SetCursor(aimCursor, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per tick
    void FixedUpdate()
    {
        //if (Input.GetButton("Fire1")) {
        //    animator.Play("Shoot");
        //    return;
        //}

        // rotate to direction
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 aimPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        float direction = -Vector2.SignedAngle(aimPosition - (Vector2)transform.position, Vector2.right);
        if (direction < 0)
            direction += 360;
        Debug.Log("angle=" + direction);

        if (direction < 45 || direction >= 360 - 45) {   // right
            animator.Play("Idle");
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction >= 180 - 45 && direction < 180 + 45) { // left
            animator.Play("Idle");
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction >= 45 && direction < 90 + 45) {  // up
            animator.Play("Back-Idle");
            transform.localScale = new Vector3(1, 1, 1);
        }
        else {  // down
            animator.Play("Front-Idle");
            transform.localScale = new Vector3(1, 1, 1);
        }

        // move
        float moveX = Input.GetAxis("Horizontal") * SPEED;
        float moveY = Input.GetAxis("Vertical") * SPEED;

        transform.position = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z);

        //if (Mathf.Abs(moveX) > 0.01)
        //    animator.Play("Walk");
        //else
        //    animator.Play("Idle");

        //  flip it
        //if (isRight && moveX < 0) {
        //    isRight = false;
        //    transform.localScale = new Vector3(-1, 1, 1);
        //} else if (!isRight && moveX > 0) {
        //    isRight = true;
        //    transform.localScale = new Vector3(1, 1, 1);
        //}
    }
}
