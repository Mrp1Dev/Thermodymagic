using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasParticles : MonoBehaviour
{
    [SerializeField] private float side;
    [SerializeField] private Transform baseSide;
    [SerializeField] private Transform piston;
    [SerializeField] private GameObject gasParticlePrefab;
    [SerializeField] private MinMax speedRange;
    [SerializeField] private Color slowColor;
    [SerializeField] private Color fastColor;
    [SerializeField] private int particlesPerMole;
    [SerializeField] private float maxMoles;

    private List<GasParticle> particles = new List<GasParticle>();
    private int currentTargetParticleCount;
    private float currentSpeed;
    private Color currentColor;
    public void SetMoles(float moles)
    {
        currentTargetParticleCount = Mathf.RoundToInt(Mathf.Clamp(moles, 0.0f, maxMoles) * particlesPerMole);
    }

    private void Update()
    {
        if (currentTargetParticleCount > particles.Count)
        {
            CreateNewParticle();
        }
        if(currentTargetParticleCount < particles.Count)
        {
            DestroyParticle();
        }
    }

    private void CreateNewParticle()
    {
        var x = Random.Range(-side, side);
        var localY = Random.Range(0.0f, piston.localPosition.y);
        var pos = new Vector3(x, localY + baseSide.transform.position.y, 0.0f);
        var obj = Instantiate(gasParticlePrefab, pos, Quaternion.identity, transform).GetComponent<GasParticle>();
        particles.Add(obj);
        obj.SetSpeed(currentSpeed);
        obj.GetComponent<SpriteRenderer>().color = currentColor;
    }

    private void DestroyParticle()
    {
        var particle = particles[0].gameObject;
        particles.RemoveAt(0);
        Destroy(particle);
    }

    public void SetParticleSpeedPercent(float lerpT)
    {
        currentSpeed = speedRange.Lerp(lerpT);
        currentColor = Color.Lerp(slowColor, fastColor, lerpT);
        foreach(var particle in particles)
        {
            particle.SetSpeed(currentSpeed);
            particle.GetComponent<SpriteRenderer>().color = currentColor;
        }
        
    }
}
