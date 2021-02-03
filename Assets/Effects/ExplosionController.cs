using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float BLAST_RADIUS = 5;
    public float BLAST_POWER = 100;

    private float CalcDamage(Vector2 position)
    {
        float distance = (position - (Vector2)transform.position).magnitude;
        if (distance > BLAST_RADIUS) 
            return 0;

        return (1 - distance / BLAST_RADIUS) * BLAST_POWER;
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
                unit.TakeDamage(damage, null);
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

        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
        effectsController.SpawnBlastMark(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
