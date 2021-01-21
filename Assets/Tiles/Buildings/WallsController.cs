using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct DestructableTile
{
    public Sprite normal;
    public Sprite destructed;
}

public class WallsController : MonoBehaviour
{
    //[SerializeField]
    public List<Tile> destruction;

    private Dictionary<Vector3Int, float> tileDamage = new Dictionary<Vector3Int, float>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealDamage(Vector3 position, float damage)
    {
        var tilemap = GetComponent<Tilemap>();
        Vector3Int tilePosition = tilemap.WorldToCell(position);
        if (tileDamage.ContainsKey(tilePosition))
            tileDamage[tilePosition] += damage;
        else
            tileDamage[tilePosition] = damage;
        if (tileDamage[tilePosition] > 20) {
            if (tilemap.GetTile(tilePosition) == destruction[0]) {
                tilemap.SetTile(tilePosition, destruction[1]);
                var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
                effectsController.RemoveBulletHoles((Vector2Int)tilePosition);
            }
        }
    }
}
