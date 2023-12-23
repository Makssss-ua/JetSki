using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetSkiWaterSplash : MonoBehaviour
{
    [SerializeField] private ParticleSystem waterSplash;
    [SerializeField] private Buoyancy buoyancy;

    private void OnEnable()
    {
        buoyancy.OnWatered += OnWatered;
        waterSplash.transform.SetParent(null);
    }

    private void OnWatered(bool watered)
    {
        if (watered)
        {
            waterSplash.Stop();
            waterSplash.transform.position = transform.position;
            waterSplash.Play();
        }
    }

    private void OnDisable()
    {
        buoyancy.OnWatered -= OnWatered;
    }
}
