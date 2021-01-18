using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : UnitController
{
    private float nextShotTime = 0;
    public float SHOT_SPEED = 0.25f;
    public float SHOT_POWER = 3;
    public float ACCURACY = 0.5f;

    private bool isDead = false;

    public GameObject ammoHUDBullet;
    public GameObject ammoHUDContainer;

    // Start is called before the first frame update
    void Start()
    {
        Init(GetComponent<Animator>());
        DrawHUDBullets();
    }

    protected override void OnDie()
    {
        isDead = true;

        var renderer = GetComponent<SpriteRenderer>();
        renderer.sortingLayerName = "DeadBodies";
    }

    protected override void OnShoot()
    {
        DrawHUDBullets();
    }

    private void DrawHUDBullets()
    {
        var children = new List<GameObject>();
        foreach (Transform child in ammoHUDContainer.transform) {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));

        for (int i = 0; i < ammo; i++)
        {
            var bullet = Instantiate(ammoHUDBullet, ammoHUDContainer.transform.position + new Vector3(10 * i, 0, 0), Quaternion.identity);
            bullet.transform.SetParent(ammoHUDContainer.transform);
        }
    }

    // Update is called once per tick
    void FixedUpdate()
    {
        if (isDead)
            return;

        // rotate to direction
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 aimPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        float direction = -Vector2.SignedAngle(aimPosition - (Vector2)transform.position, Vector2.right);
        if (direction < 0)
            direction += 360;

        string animation = "Idle";
        bool forceAnimation = false;

        // move
        float moveX = Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
        float moveY = Input.GetAxis("Vertical") * Time.fixedDeltaTime;
        Move(moveX, moveY);

        if (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveY) > 0)
            animation = "Walk";

        // shooting
        if (Input.GetButton("Fire1") && Time.fixedTime >= nextShotTime && ammo > 0) {
            animation = "Shoot";
            forceAnimation = true;
            Shoot(aimPosition + new Vector2(Random.Range(-ACCURACY, ACCURACY), Random.Range(-ACCURACY, ACCURACY)), SHOT_POWER);
            nextShotTime = Time.fixedTime + SHOT_SPEED;
        }
        else if (Time.fixedTime < nextShotTime) {
            animation = "Shoot";
            return; // NB!
        }

        // flip if necessary
        GetComponent<SpriteRenderer>().flipX = GetFlipX(direction) < 0;

        // animate
        PlayAnimatinon(GetAnimPrefix(direction) + animation, forceAnimation);

        // proper Y-sorting
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sortingOrder = (int)(-transform.position.y * 10);
    }
}
