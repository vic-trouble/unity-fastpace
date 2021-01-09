using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController: MonoBehaviour
{
    // movement speed
    public float SPEED = 1f;

    public float HEALTH = 10;

    private float health;

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
        Vector3 newPosition = new Vector3(transform.position.x + dx * SPEED, transform.position.y + dy * SPEED, transform.position.z);
        //if (CollisionController.Instance().HitTest(this, newPosition))
        //    return;

        transform.position = newPosition;
    }

    protected void Shoot(Vector3 targetPosition, float damage)
    {
        // disable self colliders
        foreach (var collider in GetComponents<Collider2D>()) {
            collider.enabled = false;
        }

        Vector3 direction = targetPosition - transform.position;
        var hit = Physics2D.Raycast(transform.position, direction, direction.magnitude);

        // enable self colliders
        foreach (var collider in GetComponents<Collider2D>()) {
            collider.enabled = true;
        }

        if (hit) {
            UnitController unit = hit.transform.gameObject.GetComponent<UnitController>();
            if (unit) {
                unit.TakeDamage(damage);
                GameObject.Find("+Effects").GetComponent<EffectsController>().SpawnSplatterEffect(hit.centroid, SplatterEffect.Blood);
            }
            else {
                GameObject.Find("+Effects").GetComponent<EffectsController>().SpawnSplatterEffect(hit.centroid, SplatterEffect.Dirt);
            }
        }
        else {
            GameObject.Find("+Effects").GetComponent<EffectsController>().SpawnSplatterEffect(targetPosition, SplatterEffect.Dirt);
        }
    }

    protected void TakeDamage(float damage)
    {
        health -= damage;

        if (health > 0) {
            PlayAnimatinon("Hit");
        }
    }

}
