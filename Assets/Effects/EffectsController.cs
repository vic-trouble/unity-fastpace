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

    public void SpawnSplatterEffect(Vector3 position, SplatterEffect effect)
    {
        var particles = Instantiate(GetEffect(effect), position, Quaternion.identity);
        particles.transform.parent = transform;
        particles.GetComponent<ParticleSystem>().Play();
    }
}
