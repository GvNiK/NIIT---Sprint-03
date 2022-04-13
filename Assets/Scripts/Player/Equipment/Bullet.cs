using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool hasFired;
    private float lifetimeState;
    private float lifetime = 2f;
    private float bulletSpeed = 5f;

    public void Fire(Transform target, bool hasTarget)
    {
        if(hasTarget)
        {
            transform.LookAt(target);
        }
        else
        {
            transform.rotation = target.rotation;
        }

        hasFired = true;

    }

    private void Update()
    {
        lifetimeState += Time.deltaTime;
        if(lifetimeState >= lifetime)
        {
            Destroy();
        }

        if (hasFired)
        {
            transform.position += transform.forward * Time.deltaTime * bulletSpeed;
        }
    }

    private void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit!");
        if (other.transform.tag.Equals("Enemy"))
        {
            
            other.transform.GetComponent<Guard>().TakeDamage(25f, this.transform);
            Destroy();
        }
    }
}

