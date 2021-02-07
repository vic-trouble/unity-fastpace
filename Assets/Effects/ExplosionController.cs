using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float BLAST_RADIUS = 5;
    public float BLAST_POWER = 100;

    public float SCREEN_SHAKE_TIME = 0.5f;
    public float SCREEN_SHAKE_MAGNITUDE = 0.5f;

    private UnitController attacker;

    private float CalcDamage(Vector2 position)
    {
        float distance = (position - (Vector2)transform.position).magnitude;
        if (distance > BLAST_RADIUS) 
            return 0;

        return (1 - distance / BLAST_RADIUS) * BLAST_POWER;
    }

    public void Init(UnitController attacker)
    {
        this.attacker = attacker;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var destructibleObject in FindObjectsOfType<DestructibleObject>()) {
            float damage = CalcDamage(destructibleObject.gameObject.transform.position);
            if (damage > 0) {
                destructibleObject.DealDamage(damage);
                Debug.Log("Dealt " + damage + " damage to " + destructibleObject);
            }
        }

        foreach (var unit in FindObjectsOfType<UnitController>()) {
            float damage = CalcDamage(unit.gameObject.transform.position);
            if (damage > 0) {
                unit.TakeDamage(damage, attacker);
                Debug.Log("Dealt " + damage + " damage to " + unit);
            }
        }

        foreach (var wallsController in FindObjectsOfType<WallsController>()) {
            for (float x = -BLAST_RADIUS; x <= BLAST_RADIUS; x += 0.5f) {
                for (float y = -BLAST_RADIUS; y <= BLAST_RADIUS; y += 0.5f) {
                    Vector3 pos = transform.position + new Vector3(x, y, 0);
                    float damage = CalcDamage(pos) / 2;
                    if (damage > 0) {
                        wallsController.DealDamage(pos, damage);
                    }
                }
            }
        }

        foreach (var dynamitePack in FindObjectsOfType<DynamitePackController>()) {
            float damage = CalcDamage(dynamitePack.transform.position);
            if (damage > 0) {
                dynamitePack.DealDamage(attacker);
            }
        }

        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
        effectsController.SpawnBlastMark(transform.position);

        var camera = FindObjectOfType<CameraController>();
        camera.ShakeScreen(SCREEN_SHAKE_TIME, SCREEN_SHAKE_MAGNITUDE);
    }

    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
