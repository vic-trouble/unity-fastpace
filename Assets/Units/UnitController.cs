using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitController: MonoBehaviour
{
    // movement speed
    public float SPEED = 1f;

    public float HEALTH = 10;

    public float health { get; private set; }

    private Animator animator;
    private string currentAnimation;

    public int AMMO = 6;

    protected int ammo;

    protected void Init(Animator animator)
    {
        ammo = AMMO;
        health = HEALTH;
        this.animator = animator;
    }

    protected void Reload()
    {
        ammo = AMMO;
    }

    protected string GetAnimPrefix(float direction)
    {
        if (direction < 45 || direction >= 360 - 45) {   // right
            return "";  // used to be 'Side' in sprites
        }
        else if (direction >= 180 - 45 && direction < 180 + 45) { // left
            return "";
        }
        else if (direction >= 45 && direction < 90 + 45) {  // up
            return "Back-";
        }
        else {  // down
            return "Front-";
        }
    }

    protected int GetFlipX(float direction)
    {
        if (direction >= 180 - 45 && direction < 180 + 45) // left
            return -1;

        return 1;
    }

    protected void PlayAnimatinon(string animation, bool force = false)
    {
        if (currentAnimation == animation && !force)
            return;

        animator.Play(animation);
        currentAnimation = animation;
    }

    // Start is called before the first frame update
    //protected void Start()
    //{
    //}

    // Update is called once per frame
    //private void Update()
    //{
    //}

    protected void Move(float dx, float dy)
    {
        transform.position = new Vector3(transform.position.x + dx * SPEED, transform.position.y + dy * SPEED, transform.position.z);
    }

    private void DisableColliders()
    {
        foreach (var collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = false;
    }

    private void EnableColliders()
    {
        foreach (var collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = true;
    }

    protected struct HitPoint
    {
        public Vector2 point;
        public GameObject gameObject;

        public HitPoint(Vector2 point)
        {
            this.point = point;
            this.gameObject = null;
        }

        public HitPoint(Vector2 point, GameObject gameObject)
        {
            this.point = point;
            this.gameObject = gameObject;
        }
    }

    protected void Shoot(Vector3 targetPosition, float damage)
    {
        // spawn bullet
        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
        effectsController.SpawnBulletTrailEffect((Vector2)transform.position + Vector2.up / 2, targetPosition, this, damage);

        ammo--;
        OnShoot();
    }

    protected virtual void OnHit(UnitController attacker)
    {
    }
    protected virtual void OnDie()
    {
    }
    protected virtual void OnShoot()
    {
    }

    public void TakeDamage(float damage, UnitController attacker)
    {
        health -= damage;

        if (health > 0) {
            PlayAnimatinon("Hit");
            OnHit(attacker);
        }
        else {
            Die();
            OnDie();
        }
    }

    protected void Die()
    {
        health = 0;
        DisableColliders();

        PlayAnimatinon("Die");
    }

}
