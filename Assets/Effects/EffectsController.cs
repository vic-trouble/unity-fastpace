using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public GameObject bloodStain;

    private List<GameObject> bulletHoles = new List<GameObject>();
    public int MAX_BULLET_HOLES = 200;

    private List<GameObject> bloodStains = new List<GameObject>();
    public int MAX_BLOOD_STAINS = 50;

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

    public GameObject Spawn(GameObject effect, Vector3 position)
    {
        return Spawn(effect, position, Quaternion.identity);
    }

    private GameObject Spawn(GameObject effect, Vector3 position, Quaternion rotation)
    {
        GameObject effectObj = Instantiate(effect, position, rotation);
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

    public void SpawnBulletTrailEffect(Vector3 start, Vector3 end, UnitController attacker, float damage)   // TODO: not true; it actually spawns bullet
    {
        start = start + (end - start).normalized * 0.25f;
        var trail = Spawn(bulletTrailEffect, start);
        trail.GetComponent<BulletController>().Init(start, end, attacker, damage);
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
            while (bulletHoles.Count >= MAX_BULLET_HOLES) {
                var bulletHole = bulletHoles[0];
                bulletHoles.RemoveAt(0);
                Destroy(bulletHole);
            }

            bulletHoles.Add(Spawn(effect, position));
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

    public void RemoveBulletHoles(Vector2Int cellPosition)
    {
        List<GameObject> toRemove = new List<GameObject>();

        var walls = GameObject.Find("Map/Buildings/Walls").GetComponent<Tilemap>();
        foreach (var bulletHole in bulletHoles) {
            if ((Vector2Int)walls.WorldToCell(bulletHole.transform.position) == cellPosition) {
                toRemove.Add(bulletHole);
            }
        }

        toRemove.ForEach(bulletHole => Destroy(bulletHole));
    }

    public void SpawnBloodStain(Vector3 position, Quaternion rotation)
    {
        while (bloodStains.Count >= MAX_BULLET_HOLES) {
            var bloodStain = bloodStains[0];
            bloodStains.RemoveAt(0);
            Destroy(bloodStain);
        }

        bloodStains.Add(Spawn(bloodStain, position, rotation));
    }
}
