using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteStickController : MonoBehaviour
{

    public float ROTATION_SPEED = 1;
    public float SPEED = 1;
    public float VERT_SPEED = 1;
    public float FUSE_TIME = 2;

    private float boomTime = 0;

    private Vector2 start, end;

    public void Init(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }

    // Start is called before the first frame update
    void Start()
    {
        var body = GetComponent<Rigidbody2D>();
        body.AddForce((end - start).normalized * SPEED + Vector2.up * VERT_SPEED, ForceMode2D.Impulse);

        boomTime = Time.fixedTime + FUSE_TIME;
    }

    void FixedUpdate()
    {
        transform.localEulerAngles += new Vector3(0, 0, ROTATION_SPEED) * Time.fixedDeltaTime;

        if (Time.fixedTime >= boomTime) {
            var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
            effectsController.SpawnExplosion(transform.position);
            Destroy(gameObject);
        }
    }
}
