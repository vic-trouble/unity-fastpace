using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct DestructibleTile
{
    public Tile tile;
    public float damageLevel;
}

[System.Serializable]
public struct DestructibleWall
{
    public List<DestructibleTile> tiles;
}

public class WallsController : MonoBehaviour
{
    public List<DestructibleWall> destructibleWalls;

    private Dictionary<Vector3Int, float> tileDamage = new Dictionary<Vector3Int, float>();

    public void DealDamage(Vector3 position, float damage)
    {
        var tilemap = GetComponent<Tilemap>();
        Vector3Int tilePosition = tilemap.WorldToCell(position);
        TileBase curTile = tilemap.GetTile(tilePosition);
        if (!curTile)
            return;

        if (tileDamage.ContainsKey(tilePosition))
            tileDamage[tilePosition] += damage;
        else
            tileDamage[tilePosition] = damage;
        float curDamage = tileDamage[tilePosition];

        foreach (var destructibleWall in destructibleWalls) {
            TileBase newTile = null;
            foreach (var destructibleTile in destructibleWall.tiles) {
                if ((SameTile(curTile, destructibleTile.tile) || newTile) && curDamage >= destructibleTile.damageLevel) {
                    newTile = destructibleTile.tile;
                }
            }
            if (newTile && !SameTile(newTile, curTile)) {
                tilemap.SetTile(tilePosition, newTile);
                var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
                effectsController.RemoveBulletHoles((Vector2Int)tilePosition);
            }
        }
    }

    private bool SameTile(TileBase tileA, TileBase tileB)
    {
        return tileA.name == tileB.name;
    }
}
