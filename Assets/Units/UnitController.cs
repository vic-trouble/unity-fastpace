using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitController : MonoBehaviour
{
    // movement speed
    public float SPEED = 1f;

    private string currentAnimation;
    private int animationCount = 0;


    protected string GetAnimPrefix(float direction)
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

    protected int GetFlipX(float direction)
    {
        if (direction >= 180 - 45 && direction < 180 + 45) // left
            return -1;

        return 1;
    }

    protected void PlayAnimatinon(Animator animator, string animation, bool force = false)
    {
        if (currentAnimation == animation && !force)
            return;

        animator.Play(animation);
        currentAnimation = animation;

        Debug.Log("Anim " + animationCount + " " + currentAnimation);
        animationCount++;
    }

    // Start is called before the first frame update
    //protected void Start()
    //{
    //}

    // Update is called once per frame
    //private void Update()
    //{
    //}

    protected void Move(float dx, float dy)
    {
        Vector3 newPosition = new Vector3(transform.position.x + dx * SPEED, transform.position.y + dy * SPEED, transform.position.z);
        //if (CollisionController.Instance().HitTest(this, newPosition))
        //    return;

        transform.position = newPosition;
    }

    protected void Shoot(Vector3 targetPosition)
    {
        GameObject.Find("+Effects").GetComponent<EffectsController>().SpawnSplatterEffect(targetPosition);
    }

}
