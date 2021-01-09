using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionController // : MonoBehaviour
{
    static CollisionController instance;

    private CollisionController()
    {
    }

    public static CollisionController Instance()
    {
        if (instance == null)
            instance = new CollisionController();
        return instance;
    }

    public bool HitTest(UnitController unit, Vector3 position)
    {
        //foreach()
        var obstaclesMap = GameObject.Find("Map/Obstacles").GetComponent<Tilemap>();
        Vector3Int cellPosition = obstaclesMap.WorldToCell(position);
        cellPosition.z = 0;
        Debug.Log("cell " + cellPosition.ToString());
        if (obstaclesMap.HasTile(cellPosition))
            return true;

        // TODO: units cross-check
        return false;
    }
}
