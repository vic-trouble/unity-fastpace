using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public GameObject dirtSplatterEffect;
    public GameObject bloodSplatterEffect;
    public GameObject bulletTrailEffect;
    
    public GameObject concreteBulletHole;
    public GameObject woodBulletHole;
    public GameObject glassBulletHole;

    public GameObject concreteDebris;
    public GameObject woodDebris;
    public GameObject glassDebris;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private GameObject GetSplatterEffect(Material material)
    {
        switch (material) {
            case Material.Dirt:
                return dirtSplatterEffect;
            case Material.Meat:
                return bloodSplatterEffect;
            default:
                return null;
        }
    }

    private GameObject Spawn(GameObject effect, Vector3 position)
    {
        GameObject effectObj = Instantiate(effect, position, Quaternion.identity);
        effectObj.transform.parent = transform;
        return effectObj;
    }

    public void SpawnSplatterEffect(Vector3 position, Material material)
    {
        var effect = GetSplatterEffect(material);
        if (effect) {
            var particles = Spawn(effect, position);
            particles.GetComponent<ParticleSystem>().Play();
        }
    }

    public void SpawnBulletTrailEffect(Vector3 start, Vector3 end)
    {
        start = start + (end - start).normalized * 0.25f;
        var trail = Spawn(bulletTrailEffect, start);
        trail.GetComponent<BulletController>().Init(start, end);
    }

    private GameObject GetBulletHoleEffect(Material material)
    {
        switch (material) {
            case Material.Wood:
                return woodBulletHole;
            case Material.Concrete:
                return concreteBulletHole;
            case Material.Glass:
                return glassBulletHole;
            default:
                return null;
        }
    }

    public void SpawnBulletHole(Vector3 position, Material material)
    {
        var effect = GetBulletHoleEffect(material);
        if (effect) {
            Spawn(effect, position);
        }
    }

    private GameObject GetDebrisEffect(Material material)
    {
        switch (material) {
            case Material.Wood:
                return woodDebris;
            case Material.Concrete:
                return concreteDebris;
            case Material.Glass:
                return glassDebris;
            default:
                return null;
        }
    }

    public void SpawnDebris(Vector3 position, Material material)
    {
        var effect = GetDebrisEffect(material);
        if (effect) {
            Spawn(effect, position);
        }
    }
}
