using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // aim cursor
    public Texture2D aimCursor;

    public GameObject followUnit;

    // Use this for initialization
    void Start ()
    {
        Cursor.SetCursor(aimCursor, new Vector2(32, 32), CursorMode.Auto);
	}


    // Update is called once per frame
    void Update()
    {
        int lerpFactor = 40;
        float x = Mathf.Lerp(transform.position.x, followUnit.transform.position.x, Time.deltaTime * lerpFactor);
        float y = Mathf.Lerp(transform.position.y, followUnit.transform.position.y, Time.deltaTime * lerpFactor);

        transform.position = new Vector3(x, y, transform.position.z); // Camera follows the player with specified offset position
    }
}
