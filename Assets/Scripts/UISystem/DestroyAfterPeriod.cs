using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterPeriod : MonoBehaviour
{
    //public ParticleSystem waveEffect; // Prefab for the cabitation effect
    public float period = 3f; // Period of the cabitation effect
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, period);
    }
}
