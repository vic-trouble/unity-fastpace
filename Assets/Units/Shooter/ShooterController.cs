using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    // primary animation
    private Animator animator;

    // movement speed
    public float SPEED = 1f;

    // aim cursor
    public Texture2D aimCursor;

    //private bool isRight = true;

    private string currentAnimation;
    private int animationCount = 0;

    private float nextShotTime = 0;
    public float SHOT_SPEED = 0.25f;

    public GameObject splatterEffect;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Cursor.SetCursor(aimCursor, new Vector2(32, 32), CursorMode.Auto);
    }

    private string GetAnimPrefix(float direction)
    {
        if (direction < 45 || direction >= 360 - 45) {   // right
            return "";  // used to be 'Side' in sprites
        }
        else if (direction >= 180 - 45 && direction < 180 + 45) { // left
            return "";
        }
        else if (direction >= 45 && direction < 90 + 45) {  // up
            return "Back-";
        }
        else {  // down
            return "Front-";
        }
    }

    private int GetFlipX(float direction)
    {
        if (direction >= 180 - 45 && direction < 180 + 45) // left
            return -1;

        return 1;
    }

    private void PlayAnimatinon(string animation)
    {
        if (currentAnimation == animation)
            return;

        animator.Play(animation);
        currentAnimation = animation;

        Debug.Log("Anim " + animationCount + " " + currentAnimation);
        animationCount++;
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

        // move
        float moveX = Input.GetAxis("Horizontal") * SPEED * Time.fixedDeltaTime;
        float moveY = Input.GetAxis("Vertical") * SPEED * Time.fixedDeltaTime;
        transform.position = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z);

        if (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveY) > 0)
            animation = "Walk";

        // shooting
        if (Input.GetButton("Fire1") && Time.fixedTime >= nextShotTime) {
            animation = "Shoot";
            nextShotTime = Time.fixedTime + SHOT_SPEED;
            var effect = Instantiate(splatterEffect, aimPosition, Quaternion.identity);
            effect.transform.parent = GameObject.Find("+Effects").transform;
            var particles = effect.GetComponent<ParticleSystem>();
            particles.Play();
            //particles.Emit();
            //var emitParams = new ParticleSystem.EmitParams();
            //emitParams.startColor = Color.red;
            //emitParams.startSize = 0.2f;
            //particles.Emit(emitParams, 10);
        }
        else if (Time.fixedTime < nextShotTime) {
            animation = "Shoot";
        }

        // flip if necessary
        transform.localScale = new Vector3(GetFlipX(direction), 1, 1);

        // animate
        PlayAnimatinon(GetAnimPrefix(direction) + animation);
    }
}
