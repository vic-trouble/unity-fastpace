﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Vector2 start, end;

    public float SPEED = 250;

    public void Init(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }

    void Start()
    {
        var body = GetComponent<Rigidbody2D>();
        body.AddForce((end - start).normalized * SPEED, ForceMode2D.Impulse);
        transform.eulerAngles = new Vector3(0, 0, Vector3.SignedAngle(new Vector3(1, 0, 0), end - start, new Vector3(0, 0, 1)));
    }

    // Update is called once per frame
    void Update()
    {
        /*
        var trail = GetComponentInChildren(typeof(TrailRenderer)) as TrailRenderer;
        if (trail.positionCount > 1) {
            Vector2 trailStart = trail.GetPosition(0), trailEnd = trail.GetPosition(trail.positionCount - 1);
            bool startReached = Vector2.Angle((end - trailStart), (end - start)) > 90;
            bool endReached = Vector2.Angle((end - trailEnd), (end - start)) > 90;

            //Debug.Log(trail.positionCount + " " + startReached + " " + endReached);

            //if (endReached) {
            //    trail.SetPosition(trail.positionCount - 1, end);
            //}
            if (endReached) {
                Destroy(gameObject);
            }
        }
        */
        if (Vector2.Angle((Vector2)transform.position - end, start - end) > 90) {
            Destroy(gameObject);
        }
    }
}
