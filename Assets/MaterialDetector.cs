using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Material
{
    None,
    Dirt,
    Meat,
    Wood,
    Concrete,
    Glass
}

public class MaterialDetector
{
    public static Material GuessMaterial(GameObject gameObject, Vector2 point)
    {
        var tileMap = gameObject.GetComponent<Tilemap>();
        if (tileMap) {
            var sprite = tileMap.GetSprite(tileMap.WorldToCell(point));
            if (sprite)
                return GuessMaterialFromTexture(sprite.texture.name);
        }

        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer) {
             if (spriteRenderer.sprite) {
                Debug.Log("texture " + spriteRenderer.sprite.texture.name);
                return GuessMaterialFromTexture(spriteRenderer.sprite.texture.name);
            }
        }

        return Material.None;
    }

    public static Material GuessMaterialFromTexture(string texture)
    {
        texture = texture.ToLower();
        if (texture.StartsWith("wood")) 
            return Material.Wood;
        else if (texture.StartsWith("concrete") || texture.StartsWith("brick")) 
            return Material.Concrete;
        else if (texture.StartsWith("glass"))
            return Material.Glass;
        return Material.None;
    }

    public static bool IsPenetrableByBullet(Material material, out float energyStopFactor)
    {
        switch (material) {
            case Material.Wood:
                energyStopFactor = 0.5f;
                return true;
            case Material.Glass:
                energyStopFactor = 0.9f;
                return true;
            case Material.None: // treat None as unobtrusive fly-through
                energyStopFactor = 1;
                return true;
            default:
                energyStopFactor = 1;
                return false;
        }
    }
}
