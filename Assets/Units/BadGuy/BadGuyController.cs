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
    public float SHOOTING_RANGE = 5;
    public float ACCURACY = 0.5f;

    private float nextShotTime = 0;
    public float SHOT_SPEED = 0.5f;
    public float SHOT_POWER = 1;

    public float AGGRAVATION_RADIUS = 3;

    private BadGuyState state = BadGuyState.Idle;
    private UnitController target;

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
        float direction = -90;
        if (target) {
            Vector2 aimPosition = (Vector2)target.transform.position;
            direction = -Vector2.SignedAngle(aimPosition - (Vector2)transform.position, Vector2.right);
            if (direction < 0)
                direction += 360;
        }

        // flip if necessary
        GetComponent<SpriteRenderer>().flipX = GetFlipX(direction) < 0;

        // animate
        string animation = "Idle";
        bool forceAnimation = false;
        if (state == BadGuyState.Aggravated) {
            if (!target || target.health <= 0) {
                state = BadGuyState.Idle;
            }
            else {
                Vector2 aimPosition = (Vector2)target.transform.position + Vector2.up / 2 + new Vector2(Random.Range(-ACCURACY, ACCURACY), Random.Range(-ACCURACY, ACCURACY));
                Vector2 delta = aimPosition - (Vector2)transform.position;
                if (delta.magnitude < SHOOTING_RANGE && Time.fixedTime >= nextShotTime) {
                    animation = "Shoot";
                    forceAnimation = true;
                    Shoot(aimPosition, SHOT_POWER);
                    nextShotTime = Time.fixedTime + SHOT_SPEED;
                }
                else if (Time.fixedTime < nextShotTime) {
                    animation = "Shoot";
                    return; // NB!
                    }
                else {
                    float moveX = Mathf.Sign(delta.x) * Time.fixedDeltaTime;
                    float moveY = Mathf.Sign(delta.y) * Time.fixedDeltaTime;
                    Move(moveX, moveY);
                    animation = "Walk";
                }
            }
        }
        PlayAnimatinon(GetAnimPrefix(direction) + animation, forceAnimation);
    }

    public void Aggravate(UnitController attacker, bool aggravateInArea)
    {
        if (state == BadGuyState.Idle)
            state = BadGuyState.Aggravated;
        target = attacker;

        if (aggravateInArea) {
            var badGuys = FindObjectsOfType<BadGuyController>();
            foreach (var badGuy in badGuys) {
                float distance = (badGuy.transform.position - transform.position).magnitude;
                if (badGuy != this && badGuy.health > 0 && distance < AGGRAVATION_RADIUS) {
                    badGuy.Aggravate(attacker, false);
                }
            }
        }
    }

    protected override void OnHit(UnitController attacker)
    {
        Aggravate(attacker, true);
    }

    protected override void OnDie()
    {
        state = BadGuyState.Dead;
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sortingLayerName = "DeadBodies";
    }
}
