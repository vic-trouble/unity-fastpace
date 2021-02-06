using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // aim cursor
    public Texture2D aimCursor;

    public GameObject followUnit;

    private float shakeStopTime = 0;
    private float shakeMagnitude = 0;

    // Use this for initialization
    void Start()
    {
        Cursor.SetCursor(aimCursor, new Vector2(32, 32), CursorMode.Auto);
	}


    // Update is called once per frame
    void Update()
    {
        int lerpFactor = 40;
        float x = Mathf.Lerp(transform.position.x, followUnit.transform.position.x, Time.deltaTime * lerpFactor);
        float y = Mathf.Lerp(transform.position.y, followUnit.transform.position.y, Time.deltaTime * lerpFactor);

        Vector3 shake = Time.fixedTime > shakeStopTime ? Vector3.zero : new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude), 0);
        transform.position = new Vector3(x, y, transform.position.z) + shake; // Camera follows the player with specified offset position
    }

    public void ShakeScreen(float shakeTime, float shakeMagnitude)
    {
        this.shakeMagnitude = shakeMagnitude;
        shakeStopTime = Time.fixedTime + shakeTime;
    }
}
