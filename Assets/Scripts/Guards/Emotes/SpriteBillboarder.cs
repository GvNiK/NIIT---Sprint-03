using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboarder : MonoBehaviour
{
    public PivotAxis pivotAxis = PivotAxis.XY;


    [Tooltip("The target the object orients towards. Defaults to camera if none is set")]
    public Transform targetTransform;

    /// <summary>
    /// Keeps the object facing the camera.
    /// </summary>
    private void Update()
    {
        // If we've switched cameras and destroyed our old one, find our new main camera
        if (targetTransform == null)
        {
            if (Camera.main.transform != null)
			{
                targetTransform = Camera.main.transform;
			}
            else
			{
                return;
			}
        }

        // Get a Vector that points from the target to the main camera.
        Vector3 directionToTarget = targetTransform.position - transform.position;

        bool useCameraAsUpVector = true;

        // Adjust for the pivot axis.
        switch (pivotAxis)
        {
            case PivotAxis.X:
                directionToTarget.x = 0.0f;
                useCameraAsUpVector = false;
                break;

            case PivotAxis.Y:
                directionToTarget.y = 0.0f;
                useCameraAsUpVector = false;
                break;

            case PivotAxis.Z:
                directionToTarget.x = 0.0f;
                directionToTarget.y = 0.0f;
                break;

            case PivotAxis.XY:
                useCameraAsUpVector = false;
                break;

            case PivotAxis.XZ:
                directionToTarget.x = 0.0f;
                break;

            case PivotAxis.YZ:
                directionToTarget.y = 0.0f;
                break;

            case PivotAxis.Free:
            default:
                // No changes needed.
                break;
        }

        // If we are right next to the camera the rotation is undefined. 
        if (directionToTarget.sqrMagnitude < 0.001f)
        {
            return;
        }

        // Calculate and apply the rotation required to reorient the object
        if (useCameraAsUpVector)
        {
            transform.rotation = Quaternion.LookRotation(-directionToTarget, targetTransform.transform.up);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(-directionToTarget);
        }
    }
}

public enum PivotAxis
{
    // Most common options
    XY,
    Y,
    // Rotate about an individual axis.
    X,
    Z,
    // Rotate about a pair of axes.
    XZ,
    YZ,
    // Rotate about all axes.
    Free
}

