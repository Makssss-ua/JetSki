using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetSkiTail : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Buoyancy buoyancy;

    private void OnEnable()
    {
        buoyancy.OnWatered += DrawTrail;
    }

    private void DrawTrail(bool watered)
    {
        if (watered)
        {
            trailRenderer.enabled = true;
        }
        else
        {
            trailRenderer.enabled = false;
        }
    }

    private void OnDisable()
    {
        buoyancy.OnWatered -= DrawTrail;
    }
}
