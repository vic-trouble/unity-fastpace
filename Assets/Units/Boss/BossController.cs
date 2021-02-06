using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Idle,
    Aggravated,
    Dead
}

public class BossController : UnitController
{
    public float SHOOTING_RANGE = 5;
    public float ACCURACY = 0.5f;

    private float nextShotTime = 0;
    public float SHOT_SPEED = 0.5f;
    public float SHOT_POWER = 1;

    public float AGGRAVATION_RADIUS = 3;

    private BossState state = BossState.Idle;
    private UnitController target;

    // Start is called before the first frame update
    void Start()
    {
        Init(GetComponent<Animator>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state == BossState.Dead)
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
        if (state == BossState.Aggravated) {
            if (!target || target.health <= 0) {
                state = BossState.Idle;
            }
            else {
                Vector2 aimPosition = (Vector2)target.transform.position + Vector2.up / 2;

                float inaccuracyFactor = 1 - ACCURACY;
                Vector2 inaccuracy = new Vector2(Random.Range(-inaccuracyFactor, inaccuracyFactor), Random.Range(-inaccuracyFactor, inaccuracyFactor)) / 2;
                aimPosition += inaccuracy * (aimPosition - (Vector2)transform.position).magnitude;

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
        if (state == BossState.Idle)
            state = BossState.Aggravated;
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

        var healthBar = GameObject.Find("HUD").GetComponent<HUDController>().bossHealthBar.GetComponent<BossHealthBarController>();
        healthBar.Show();
        healthBar.SetHealthPortion(health / HEALTH);
    }

    protected override void OnDie()
    {
        state = BossState.Dead;
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sortingLayerName = "DeadBodies";

        var healthBar = GameObject.Find("HUD").GetComponent<HUDController>().bossHealthBar.GetComponent<BossHealthBarController>();
        healthBar.Hide();
    }
}
