using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePickup : MonoBehaviour
{
    public Vector3 RotateSpeed;
    public float BobDistance;

    private Vector3 newPos;
    private float originalY;
    public bool IsAnimating;

    void Start()
    {
        originalY = transform.localPosition.y;
        IsAnimating = true;
    }

    void Update()
    {
        if(IsAnimating)
        {
            // rotate the object
            transform.localEulerAngles += RotateSpeed;

            // use a 'sin' bob on the y position
            newPos = transform.position;
            newPos.y = originalY + (Mathf.Sin(Time.time * 4) * BobDistance);
            transform.position = newPos;
        }
    }

    public void ResetHeight()
    {
        transform.parent.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }
}
