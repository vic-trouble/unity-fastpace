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

    protected void Init(Animator animator)
    {
        health = HEALTH;
        this.animator = animator;
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
    protected void Shoot(Vector3 targetPosition, float damage)
    {
        DisableColliders();

        Vector3 direction = targetPosition - transform.position;
        var hit = Physics2D.Raycast(transform.position, direction, direction.magnitude);

        EnableColliders();

        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
        effectsController.SpawnBulletTrailEffect((Vector2)transform.position + Vector2.up / 2, targetPosition);
        if (hit) {
            UnitController unit = hit.transform.gameObject.GetComponent<UnitController>();
            if (unit) {
                unit.TakeDamage(damage, this);
                effectsController.SpawnSplatterEffect(hit.point, Material.Meat);
            }
            else {
                Vector2 probePoint = hit.point + (Vector2)direction.normalized * 0.05f;
                Material material = MaterialDetector.GuessMaterial(hit.transform.gameObject, probePoint);
                Vector2 effectPos = hit.point + (Vector2)direction.normalized * Random.Range(0.1f, 0.9f);
                effectsController.SpawnBulletHole(effectPos, material);
                effectsController.SpawnDebris(effectPos, material);
            }
        }
        else {
            var tileMap = GameObject.Find("Map/Ground").GetComponent<Tilemap>();
            string texture = tileMap.GetSprite(tileMap.WorldToCell(targetPosition)).texture.name;
            Material material = MaterialDetector.GuessMaterialFromTexture(texture);
            effectsController.SpawnSplatterEffect(targetPosition, material != Material.None ? material : Material.Dirt);
        }
    }

    protected virtual void OnHit(UnitController attacker)
    {
    }
    protected virtual void OnDie()
    {
    }

    protected void TakeDamage(float damage, UnitController attacker)
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
