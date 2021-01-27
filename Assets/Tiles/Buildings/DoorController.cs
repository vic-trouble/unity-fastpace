using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorController : MonoBehaviour
{
    private enum State
    {
        Close,
        Opening,
        Open
    }

    private State state = State.Close;

    private bool open = false;

    private float disableColliderTime = 0;

    void FixedUpdate()
    {
        if (disableColliderTime != 0 && Time.fixedTime > disableColliderTime) {
            GetComponent<Collider2D>().enabled = false;
            disableColliderTime = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<UnitController>()) {
            if (!open) {
                open = false;
                GetComponent<Animator>().SetBool("Open", true);
                disableColliderTime = Time.fixedTime + 0.2f;
                
                var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
                var walls = GameObject.Find("Map/Buildings/Walls").GetComponent<Tilemap>();
                Vector3Int tilePosition = walls.WorldToCell(transform.position);
                effectsController.RemoveBulletHoles((Vector2Int)tilePosition);
            }
        }
    }
}
