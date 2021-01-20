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
        if (gameObject.tag == "wall") {
            var tileMap = gameObject.GetComponent<Tilemap>();
            var sprite = tileMap.GetSprite(tileMap.WorldToCell(point));
            if (sprite)
                return GuessMaterialFromTexture(sprite.texture.name);
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
}
