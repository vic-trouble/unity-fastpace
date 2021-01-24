using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BulletController : MonoBehaviour
{
    private Vector2 start, end;
    private UnitController attacker;
    private float damage;

    public float SPEED = 250;

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

    // Update is called once per frame
    void Update()
    {
        /*
        var trail = GetComponentInChildren(typeof(TrailRenderer)) as TrailRenderer;
        if (trail.positionCount > 1) {
            Vector2 trailStart = trail.GetPosition(0), trailEnd = trail.GetPosition(trail.positionCount - 1);
            bool startReached = Vector2.Angle((end - trailStart), (end - start)) > 90;
            bool endReached = Vector2.Angle((end - trailEnd), (end - start)) > 90;

            //Debug.Log(trail.positionCount + " " + startReached + " " + endReached);

            //if (endReached) {
            //    trail.SetPosition(trail.positionCount - 1, end);
            //}
            if (endReached) {
                Destroy(gameObject);
            }
        }
        */
        if (Vector2.Angle((Vector2)transform.position - end, start - end) > 90) {
            Debug.Log("bullet hit the end");

            var tileMap = GameObject.Find("Map/Ground").GetComponent<Tilemap>();
            string texture = tileMap.GetSprite(tileMap.WorldToCell(transform.position)).texture.name;
            Material material = MaterialDetector.GuessMaterialFromTexture(texture);

            var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
            effectsController.SpawnSplatterEffect(transform.position, material != Material.None ? material : Material.Dirt);

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("collision on " + collider.gameObject.name);

        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();

        UnitController unit = collider.gameObject.GetComponent<UnitController>();
        if (!unit) {
            unit = collider.transform.parent.gameObject.GetComponent<UnitController>(); // TODO: this is pretty ugly
        }

        if (unit == attacker) { // you can't shoot yourself
            return;
        }

        if (unit) {
            unit.TakeDamage(damage, attacker);
            effectsController.SpawnSplatterEffect(transform.position, Material.Meat);

            Destroy(gameObject);
        }
        else {
            Vector2 direction = end - start;
            Vector2 probePoint = (Vector2)transform.position + (Vector2)direction.normalized * 0.05f;
            Material material = MaterialDetector.GuessMaterial(collider.gameObject, probePoint);
            Vector2 effectPos = (Vector2)transform.position + (Vector2)direction.normalized * Random.Range(0.1f, 0.9f);
            effectsController.SpawnBulletHole(effectPos, material);
            effectsController.SpawnDebris(effectPos, material);

            float energyStopFactor;
            bool stopBullet = !MaterialDetector.IsPenetrableByBullet(material, out energyStopFactor);
            this.damage *= energyStopFactor;

            var wallsController = collider.gameObject.GetComponent<WallsController>();
            if (wallsController) {
                wallsController.DealDamage(effectPos, damage);
            }

            if (stopBullet) {
                Destroy(gameObject);
            }
        }

    }
}
