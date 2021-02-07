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
    public AudioClip sfx;

    private float curDamage = 0;

    public void DealDamage(float damage)
    {
        curDamage += damage;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite curSprite = spriteRenderer.sprite;

        Sprite newSprite = null;
        bool oblitirated = false;
        foreach (var phase in destructionPhases) {
            oblitirated = false;
            if (curDamage >= phase.damageLevel) {
                newSprite = phase.sprite;
                oblitirated = true;
            }
        }
        if (newSprite && !SameSprite(newSprite, curSprite)) {
            spriteRenderer.sprite = newSprite;

            if (oblitirated) {
                GetComponent<Collider2D>().enabled = false;
            }

            if (destructionEffect) {
                var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
                effectsController.Spawn(destructionEffect, transform.position);
            }

            PlaySFX(sfx);
        }

    }

    private bool SameSprite(Sprite spriteA, Sprite spriteB)
    {
        return spriteA.name == spriteB.name;
    }

    private void PlaySFX(AudioClip sfx)
    {
        if (sfx) {
            var audio = GetComponent<AudioSource>();
            audio.clip = sfx;
            audio.Play();
        }
    }
}
