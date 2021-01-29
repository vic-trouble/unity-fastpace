using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DestructibleObjectPhase
{
    public Sprite sprite;
    public float damageLevel;
}

public class DestructibleObject : MonoBehaviour
{
    public List<DestructibleObjectPhase> destructionPhases;
    public GameObject destructionEffect;

    private float curDamage = 0;

    public void DealDamage(float damage)
    {
        curDamage += damage;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite curSprite = spriteRenderer.sprite;

        Sprite newSprite = null;
        foreach (var phase in destructionPhases) {
            if (curDamage >= phase.damageLevel) {
                newSprite = phase.sprite;
            }
        }
        if (newSprite && !SameSprite(newSprite, curSprite)) {
            spriteRenderer.sprite = newSprite;
            if (destructionEffect) {
                var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
                effectsController.Spawn(destructionEffect, transform.position);
            }
        }

    }

    private bool SameSprite(Sprite spriteA, Sprite spriteB)
    {
        return spriteA.name == spriteB.name;
    }
}
