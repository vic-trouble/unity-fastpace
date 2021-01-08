using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Vector2 offset;

    // Use this for initialization
    void Start ()
    {
        offset = new Vector2(0, 0);
	}


    // Update is called once per frame
    void Update()
    {
        Transform player = GameObject.Find("Shooter").transform;

        int lerpFactor = 40;
        float x = Mathf.Lerp(transform.position.x, player.position.x + offset.x, Time.deltaTime * lerpFactor);
        float y = Mathf.Lerp(transform.position.y, player.position.y + offset.y, Time.deltaTime * lerpFactor);

        transform.position = new Vector3(x, y, transform.position.z); // Camera follows the player with specified offset position
    }
}
