using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GuardVision
{
	public Action<List<GameObject>> OnObjectsInView = delegate { };
	public Action<List<GameObject>> OnObjectsInAwarenessZone = delegate { };
	public Action OnNoObjectInView = delegate { };
	private List<GameObject> objectsInCone;
	private List<GameObject> objectsInAwarenessZone;
	private Guard.VisionData visionData;
	private Guard guard;
	private List<GameObject> objectsToCheckAgainst;
    private bool visionEnabled = true;

	public GuardVision(Guard guard, Guard.VisionData visionData,
		List<GameObject> objectsToCheckAgainst)
	{
		this.guard = guard;
		this.visionData = visionData;
		this.objectsToCheckAgainst = objectsToCheckAgainst;
		objectsInCone = new List<GameObject>();
		objectsInAwarenessZone = new List<GameObject>();
	}

    public void Update()
    {
        if(visionEnabled)
        {
            CheckConeForObjects();
			CheckAwarenessZone();
        }
        
        if (objectsInCone.Count > 0)
        {
			OnObjectsInView(objectsInCone);
        }
		else if(objectsInAwarenessZone.Count > 0)
		{
			OnObjectsInAwarenessZone(objectsInAwarenessZone);
		}
        else
        {
			OnNoObjectInView();
        }

        if (visionData.visualise && Application.isEditor)
		{
			DrawCone();
			DrawAwarenessZone();
		}
    }

    private void CheckConeForObjects()
    {
        Vector3 eyePosition = new Vector3(guard.transform.position.x, guard.transform.position.y + visionData.eyeHeight, guard.transform.position.z);

        objectsInCone.Clear(); 
        foreach (GameObject item in objectsToCheckAgainst)
        {
            Vector3 direction = item.transform.position - eyePosition;
            float angle = Vector3.Angle(direction, guard.transform.forward);
            if (direction.magnitude < visionData.detectionRadius && angle < visionData.detectionAngle)
            {
				float checkDistance = Vector3.Distance(guard.transform.position, item.transform.position);

				// Make sure we have line of sight to the object (Nothing blocking the way)
				if (!Physics.Raycast(eyePosition, direction, checkDistance, visionData.raycastMask))
				{
					objectsInCone.Add(item);
				}
			}
        }
    }

	private void CheckAwarenessZone()
	{
		objectsInAwarenessZone.Clear();
		foreach (GameObject item in objectsToCheckAgainst)
		{
			float dist = Vector3.Distance(item.transform.position, guard.transform.position);

			if(dist < visionData.awarenessZone)
			{
				objectsInAwarenessZone.Add(item);
			}
		}
	}

    private void DrawCone()
	{
        // Draw outer lines
        Vector3 scaledForward = guard.transform.forward * visionData.detectionRadius;
        Vector3 rotatedForward = Quaternion.Euler(0, visionData.detectionAngle, 0) * scaledForward;
        Debug.DrawRay(guard.transform.position, rotatedForward, Color.white);
        rotatedForward = Quaternion.Euler(0, -visionData.detectionAngle, 0) * scaledForward;
        Debug.DrawRay(guard.transform.position, rotatedForward, Color.white);
        
        // Draw inner lines
        var rayColor = objectsInCone.Count <= 0 ? Color.white : Color.red;
        int iterations = ((int)(visionData.detectionAngle / 5)) * 2;
		for (int i = 1; i < iterations; i++)
		{
            float rotationAmount = visionData.detectionAngle / iterations * 2 * i - visionData.detectionAngle;
            rotatedForward = Quaternion.Euler(0, rotationAmount, 0) * scaledForward;
            Debug.DrawRay(guard.transform.position, rotatedForward, rayColor);
		}
	}

	private void DrawAwarenessZone()
	{
		for(int i = 0; i < 36; i++)
		{
			Vector3 endPoint = Quaternion.Euler(0, i * 10f, 0) * new Vector3(0f, 0f, visionData.awarenessZone);
			Debug.DrawLine(guard.transform.position, guard.transform.position + endPoint, Color.blue);
		}
	}

	public void Enable()
    {
        visionEnabled = true;
    }

    public void Disable()
    {
        visionEnabled = false;
    }

    [System.Serializable]
    public class UnityEventGameObjectList : UnityEvent<List<GameObject>> { }
}
