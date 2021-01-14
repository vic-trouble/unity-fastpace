using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SplatterEffect
{
    Dirt,
    Blood
}

public class EffectsController : MonoBehaviour
{
    public GameObject dirtSplatterEffect;
    public GameObject bloodSplatterEffect;
    public GameObject bulletTrailEffect;
    public GameObject concreteBulletHole;
    public GameObject woodBulletHole;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private GameObject GetEffect(SplatterEffect effect)
    {
        switch (effect) {
            case SplatterEffect.Dirt:
                return dirtSplatterEffect;
            case SplatterEffect.Blood:
                return bloodSplatterEffect;
        }
        return null;
    }

    private GameObject Spawn(GameObject effect, Vector3 position)
    {
        GameObject effectObj = Instantiate(effect, position, Quaternion.identity);
        effectObj.transform.parent = transform;
        return effectObj;
    }

    public void SpawnSplatterEffect(Vector3 position, SplatterEffect effect)
    {
        var particles = Spawn(GetEffect(effect), position);
        particles.GetComponent<ParticleSystem>().Play();
    }

    public void SpawnBulletTrailEffect(Vector3 start, Vector3 end)
    {
        start = start + (end - start).normalized * 0.25f;
        var trail = Spawn(bulletTrailEffect, start);
        trail.GetComponent<BulletController>().Init(start, end);
    }

    public void SpawnBulletHole(Vector3 position)
    {
        var hole = Spawn(concreteBulletHole, position);
    }
}
