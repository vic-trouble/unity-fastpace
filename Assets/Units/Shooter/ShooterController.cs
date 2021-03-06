﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : UnitController
{
    private float nextShotTime = 0;
    private float nextReloadTime = 0;   // TODO: make some timer?
    private float nextGrenadeTime = 0;

    public float SHOT_SPEED = 0.25f;
    public float SHOT_POWER = 3;
    public float ACCURACY = 0.5f;
    public float RELOAD_SPEED = 0.5f;

    public GameObject dynamite;
    public float THROW_GRENADE_COOLDOWN = 2;

    private bool isDead = false;

    public int ammoGrenades = 0;

    public AudioClip sfxPistolShot;
    public AudioClip sfxPistolReload;
    public AudioClip sfxHurt;
    public AudioClip sfxDied;
    public AudioClip sfxPowerUp;

    // Start is called before the first frame update
    void Start()
    {
        Init(GetComponent<Animator>());
        UpdateHUDAmmo();
        UpdateHUDGrenades();
    }

    protected override void OnDie()
    {
        isDead = true;

        var healthBar = GameObject.Find("HealthBar").GetComponent<HealthBarController>();
        healthBar.SetHealthPortion(0);

        var renderer = GetComponent<SpriteRenderer>();
        renderer.sortingLayerName = "DeadBodies";

        PlaySFX(sfxDied);
    }

    protected override void OnHit(UnitController attacker)
    {
        var healthBar = GameObject.Find("HealthBar").GetComponent<HealthBarController>();
        healthBar.SetHealthPortion(health / HEALTH);

        PlaySFX(sfxHurt);
    }

    private void UpdateHUDAmmo()
    {
        var ammoBar = GameObject.Find("AmmoBar").GetComponent<AmmoBarController>();
        ammoBar.SetAmmo(ammo);        
    }

    protected override void OnShoot()
    {
        UpdateHUDAmmo();
        PlaySFX(sfxPistolShot);
    }

    private void UpdateHUDGrenades()
    {
        var grenadesBar = GameObject.Find("GrenadesBar").GetComponent<GrenadesBarController>();
        grenadesBar.SetGrenades(ammoGrenades);
    }

    private void StartReload()
    {
        // no animation yet
        if (nextReloadTime == 0) {
            nextReloadTime = Time.fixedTime + RELOAD_SPEED;
            PlaySFX(sfxPistolReload);
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
        if (Input.GetButton("Fire1") && Time.fixedTime >= nextShotTime) {
            if (ammo > 0) {
                animation = "Shoot";
                forceAnimation = true;
                float inaccuracyFactor = 1 - ACCURACY;
                Vector2 inaccuracy = new Vector2(Random.Range(-inaccuracyFactor, inaccuracyFactor), Random.Range(-inaccuracyFactor, inaccuracyFactor)) / 2;
                aimPosition += inaccuracy * (aimPosition - (Vector2)transform.position).magnitude;
                Shoot(aimPosition, SHOT_POWER);
                nextShotTime = Time.fixedTime + SHOT_SPEED;
            }
            else {
                StartReload();
            }
        }
        else if (Time.fixedTime < nextShotTime) {
            animation = "Shoot";
            return; // NB!
        }
        else if (Input.GetButton("Fire2") && Time.fixedTime >= nextGrenadeTime) {
            if (ammoGrenades > 0) {
                ThrowGrenade(aimPosition);
                nextGrenadeTime = Time.fixedTime + THROW_GRENADE_COOLDOWN;
            }
        }

        if (Input.GetKey(KeyCode.R) && ammo < AMMO) {
            StartReload();
        }

        // reload
        if (nextReloadTime != 0 && Time.fixedTime >= nextReloadTime) {
            Reload();
            UpdateHUDAmmo();
            nextReloadTime = 0;
        }

        // flip if necessary
        GetComponent<SpriteRenderer>().flipX = GetFlipX(direction) < 0;

        // animate
        PlayAnimatinon(GetAnimPrefix(direction) + animation, forceAnimation);
    }

    private void ThrowGrenade(Vector2 position)
    {
        Vector3 start = transform.position + new Vector3(0, 0.5f, 0);
        var projectile = Instantiate(dynamite, start, Quaternion.identity);
        var dynamiteController = projectile.GetComponent<DynamiteStickController>();
        dynamiteController.Init(start, position, this);

        ammoGrenades--;
        UpdateHUDGrenades();
    }

    public void PickUpGrenades(int numGrenades)
    {
        ammoGrenades += numGrenades;
        UpdateHUDGrenades();

        PlaySFX(sfxPowerUp);
    }

    private void PlaySFX(AudioClip sfx)
    {
        if (sfx) {
            var audio = GetComponent<AudioSource>();
            audio.clip = sfx;
            audio.Play();
        }
    }
}
