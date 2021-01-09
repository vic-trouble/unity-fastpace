using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public GameObject splatterEffect;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnSplatterEffect(Vector3 position)
    {
        var effect = Instantiate(splatterEffect, position, Quaternion.identity);
        effect.transform.parent = transform;
        var particles = effect.GetComponent<ParticleSystem>();
        particles.Play();
        //particles.Emit();
        //var emitParams = new ParticleSystem.EmitParams();
        //emitParams.startColor = Color.red;
        //emitParams.startSize = 0.2f;
        //particles.Emit(emitParams, 10);
    }
}
