using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleController : MonoBehaviour
{
    public List<Sprite> sprites;

    // Start is called before the first frame update
    void Start()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[(int)Random.Range(0, sprites.Count - 1)];
    }
}
