﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BulletController : MonoBehaviour
{
    private Vector2 start, end;
    private UnitController attacker;
    private float damage;

    public float SPEED = 250;
    public int CRITICAL_MULTIPLIER = 5;

    private int shotThru = 0;
    private int MAX_SHOT_THRU = 2;

    public void Init(Vector2 start, Vector2 end, UnitController attacker, float damage)
    {
        this.start = start;
        this.end = end;
        this.attacker = attacker;
        this.damage = damage;
    }

    void Start()
    {
        var body = GetComponent<Rigidbody2D>();
        body.AddForce((end - start).normalized * SPEED, ForceMode2D.Impulse);
        transform.eulerAngles = new Vector3(0, 0, Vector3.SignedAngle(new Vector3(1, 0, 0), end - start, new Vector3(0, 0, 1)));
    }

    void FixedUpdate()
    {
        // speculative collision - for penetrating materials
        Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
        var hit = Physics2D.Raycast((Vector2)transform.position + velocity.normalized * 0.1f, velocity, velocity.magnitude * Time.fixedDeltaTime);
        if (hit) {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.tag == "wall") {
                var hits = Physics2D.RaycastAll(hit.point + velocity.normalized * 2, -velocity, 2);
                foreach (var subhit in hits) {
                    if (subhit.collider.gameObject == hitObject) {
                        HitWall(hitObject, hit.point, subhit.point);
                        break;
                    }
                }
            }
        }

        // reached the target
        if (Vector2.Angle((Vector2)transform.position - end, start - end) > 90) {
            var tileMap = GameObject.Find("Map/Ground").GetComponent<Tilemap>();
            var sprite = tileMap.GetSprite(tileMap.WorldToCell(transform.position));
            if (sprite) {
                string texture = sprite.texture.name;
                Material material = MaterialDetector.GuessMaterialFromTexture(texture);
                var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
                effectsController.SpawnSplatterEffect(transform.position, material != Material.None ? material : Material.Dirt);
            }

            Destroy(gameObject);
        }
    }

    private void HitWall(GameObject wall, Vector2 enterPoint, Vector2 exitPoint)
    {
        Vector2 direction = (end - start).normalized;
        Vector2 probePoint = (enterPoint + exitPoint) / 2;
        Material material = MaterialDetector.GuessMaterial(wall, probePoint);
        Debug.Log("hit material " + material);

        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
        Vector2 effectPos = (Vector2)transform.position + direction * Random.Range(0.1f, 0.9f);
        effectsController.SpawnBulletHole(effectPos, material);
        effectsController.SpawnDebris(effectPos, material);

        float energyStopFactor;
        bool stopBullet = !MaterialDetector.IsPenetrableByBullet(material, out energyStopFactor);
        this.damage *= energyStopFactor;

        if (!stopBullet) {
            stopBullet = ++shotThru >= MAX_SHOT_THRU;
        }

        var wallsController = wall.GetComponent<WallsController>();
        if (wallsController) {
            wallsController.DealDamage(effectPos, damage);
        }

        if (stopBullet) {
            Debug.Log("stop bullet with hit count " + shotThru);
            Destroy(gameObject);
        }
        else {
            Debug.Log("teleport wall");
            transform.position = exitPoint + direction * 0.1f;
        }
    }

    private void HitUnit(UnitController unit, bool isCritical)
    {
        unit.TakeDamage(damage * (isCritical ? CRITICAL_MULTIPLIER : 1), attacker);
    
        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
        if (isCritical) {
            effectsController.SpawnSplatterEffect(transform.position, Material.Meat);
            effectsController.SpawnSplatterEffect(transform.position, Material.Meat);
            effectsController.SpawnSplatterEffect(transform.position, Material.Meat);
        }
        else {
            effectsController.SpawnSplatterEffect(transform.position, Material.Meat);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
        Gizmos.DrawLine((Vector2)transform.position + velocity.normalized * 0.1f, (Vector2)transform.position + velocity.normalized * 0.1f + velocity * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("bullet collision on " + collision.gameObject.name);

        UnitController unit = collision.gameObject.GetComponent<UnitController>();
        if (unit && unit != attacker) {
            HitUnit(unit, collision.collider.tag == "Critical");
        }
    }
}
