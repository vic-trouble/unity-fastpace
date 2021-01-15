﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitController: MonoBehaviour
{
    // movement speed
    public float SPEED = 1f;

    public float HEALTH = 10;

    public float health { get; private set; }

    private Animator animator;
    private string currentAnimation;

    protected void Init(Animator animator)
    {
        health = HEALTH;
        this.animator = animator;
    }

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

    protected void PlayAnimatinon(string animation, bool force = false)
    {
        if (currentAnimation == animation && !force)
            return;

        animator.Play(animation);
        currentAnimation = animation;
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

    private void DisableColliders()
    {
        foreach (var collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = false;
    }

    private void EnableColliders()
    {
        foreach (var collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = true;
    }
    protected void Shoot(Vector3 targetPosition, float damage)
    {
        DisableColliders();

        Vector3 direction = targetPosition - transform.position;
        var hit = Physics2D.Raycast(transform.position, direction, direction.magnitude);

        EnableColliders();

        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
        effectsController.SpawnBulletTrailEffect((Vector2)transform.position + Vector2.up / 2, targetPosition);
        if (hit) {
            UnitController unit = hit.transform.gameObject.GetComponent<UnitController>();
            if (unit) {// TODO: rework with tags!
                unit.TakeDamage(damage, this);
                effectsController.SpawnSplatterEffect(hit.point, SplatterEffect.Blood);
            }
            else if (hit.transform.gameObject.tag == "wall") {
                if (/*Vector2.Angle(hit.normal, direction) < 45*/true) {
                    Debug.DrawRay(hit.point, hit.normal.normalized * 0.2f, Color.white, 2);
                    var tileMap = hit.transform.gameObject.GetComponent<Tilemap>();
                    string texture = tileMap.GetSprite(tileMap.WorldToCell(hit.point + (Vector2)direction.normalized * 0.1f)).texture.name;
                    Debug.Log("texture " + texture);
                    bool isWood = texture == "wooden-wall";
                    Vector2 effectPos = hit.point + (Vector2)direction.normalized * Random.Range(0.1f, 0.9f);
                    effectsController.SpawnBulletHole(effectPos, isWood);
                    effectsController.SpawnDebris(effectPos, isWood);
                }
            }
            else {
                effectsController.SpawnSplatterEffect(hit.point, SplatterEffect.Dirt);
            }
        }
        else {
            effectsController.SpawnSplatterEffect(targetPosition, SplatterEffect.Dirt);
        }
    }

    protected virtual void OnHit(UnitController attacker)
    {
    }
    protected virtual void OnDie()
    {
    }

    protected void TakeDamage(float damage, UnitController attacker)
    {
        health -= damage;

        if (health > 0) {
            PlayAnimatinon("Hit");
            OnHit(attacker);
        }
        else {
            Die();
            OnDie();
        }
    }

    protected void Die()
    {
        health = 0;
        DisableColliders();

        PlayAnimatinon("Die");
    }

}
