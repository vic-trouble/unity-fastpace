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
    public float MIN_SPEED = 3;
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
        PrePenetrate();

        // destroy stopped bullets
        if (GetComponent<Rigidbody2D>().velocity.magnitude < MIN_SPEED) {
            Destroy(gameObject);
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

        // find effect position
        Vector2 effectPos = (Vector2)transform.position;
        var wallTilemap = wall.GetComponent<Tilemap>();
        if (wallTilemap) {
            Vector3Int hitCell = wallTilemap.WorldToCell(probePoint);
            var cellSprite = wallTilemap.GetSprite(hitCell);
            if (cellSprite) {
                for (int i = -5; i < 5; i++) { // try 10 random adjustments
                    Vector2 bestEffectPos = (Vector2)probePoint + direction * Random.Range(0.1f, 0.9f) * (i < 0 ? -1 : 1);
                    Vector3Int cell = wallTilemap.WorldToCell(bestEffectPos);
                    if (!cell.Equals(hitCell)) {
                        continue;
                    }

                    Vector2 texturePos = (bestEffectPos - (Vector2)wallTilemap.CellToWorld(cell)) * cellSprite.pixelsPerUnit;
                    Color hitColor = cellSprite.texture.GetPixel((int)texturePos.x, (int)texturePos.y);
                    if (hitColor.a > 0.5) {
                        effectPos = bestEffectPos;
                        break;
                    }
                }
            }
        }

        // debris splatter effect
        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
        effectsController.SpawnDebris(effectPos, material);

        // hit the wall
        var wallsController = wall.GetComponent<WallsController>();
        if (wallsController) {
            wallsController.DealDamage(effectPos, damage);
        }

        // penetration or full stop
        bool penetrated = Penetrate(material, enterPoint, exitPoint);

        // bullet hole effect
        if (Vector2.Angle(direction, Vector2.up) < 90 || penetrated) {
            if (wall.tag == "wall") {
                effectsController.SpawnBulletHole(effectPos, material);
            }
        }
    }

    private void PrePenetrate()
    {
        // speculative collision - for penetrating materials
        Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
        var hit = Physics2D.Raycast((Vector2)transform.position + velocity.normalized * 0.1f, velocity, velocity.magnitude * Time.fixedDeltaTime);
        if (hit) {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.tag == "wall" || hitObject.tag == "SideWall") {
                var hits = Physics2D.RaycastAll(hit.point + velocity.normalized * 2, -velocity, 2);
                foreach (var subhit in hits) {
                    if (subhit.collider.gameObject == hitObject) {
                        HitWall(hitObject, hit.point, subhit.point);
                        break;
                    }
                }
            }
            else if (hitObject.tag == "Penetrable") {
                var hits = Physics2D.RaycastAll(hit.point + velocity.normalized * 2, -velocity, 2);
                foreach (var subhit in hits) {
                    if (subhit.collider.gameObject == hitObject) {
                        HitObject(hitObject.GetComponent<DestructibleObject>(), hit.point, subhit.point);
                        break;
                    }
                }
            }
        }
    }

    private bool Penetrate(Material material, Vector2 enterPoint, Vector2 exitPoint)
    {
        float energyStopFactor;
        bool stopBullet = !MaterialDetector.IsPenetrableByBullet(material, out energyStopFactor);
        this.damage *= energyStopFactor;
        if (!stopBullet) {
            stopBullet = ++shotThru >= MAX_SHOT_THRU;
        }

        if (stopBullet) {
            Debug.Log("stop bullet with hit count " + shotThru);
            Destroy(gameObject);
        }
        else {
            Vector2 direction = (end - start).normalized;
            transform.position = exitPoint + direction * 0.1f;
            PrePenetrate();
        }

        return !stopBullet;
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

    void HitObject(DestructibleObject destructibleObject, Vector2 enterPoint, Vector2 exitPoint)
    {
        destructibleObject.DealDamage(damage);

        Vector2 probePoint = (enterPoint + exitPoint) / 2;
        Material material = MaterialDetector.GuessMaterial(destructibleObject.gameObject, probePoint);
        Debug.Log("hit material " + material);

        Penetrate(material, enterPoint, exitPoint);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("bullet collision on " + collision.gameObject.name);

        UnitController unit = collision.gameObject.GetComponent<UnitController>();
        if (unit && unit != attacker) {
            HitUnit(unit, collision.collider.tag == "Critical");
        }
        /*
        var destructibleObject = collision.gameObject.GetComponent<DestructibleObject>();
        if (destructibleObject) {
            HitObject(destructibleObject);
        }
        */
    }
}
