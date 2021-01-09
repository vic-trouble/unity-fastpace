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
        // TODO: this should be replaced with standard colliders
        var obstaclesMap = GameObject.Find("Map/Obstacles").GetComponent<Tilemap>();
        Vector3Int cellPosition = obstaclesMap.WorldToCell(position);
        cellPosition.z = 0;
        if (obstaclesMap.HasTile(cellPosition))
            return true;

        foreach (var testUnit in GameObject.FindObjectsOfType<UnitController>()) {
            Debug.DrawLine(testUnit.transform.position, testUnit.transform.position + new Vector3(1, 0, 0));
            Debug.DrawLine(testUnit.transform.position, testUnit.transform.position - new Vector3(1, 0, 0));
            // do not test against itself
            if (testUnit == unit)
                continue;

            Vector3 distance = testUnit.transform.position - unit.transform.position;
            if (distance.magnitude < 1) {
                Debug.Log("collided with " + testUnit.ToString() + " , " + distance.magnitude);
                return true;
            }
        }
        return false;
    }
}
