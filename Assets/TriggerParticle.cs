using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerParticle : MonoBehaviour
{
    public ParticleSystem particlSystem;
    public bool isAmmo;
    public Color ammoColor;
    public Color weaponColor;

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        particlSystem.Play();
        var main = particlSystem.main;

        main.startColor = isAmmo ? ammoColor : weaponColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        particlSystem.Stop();
    }

}
