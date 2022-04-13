using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewOcclusionManager
{
    private const float SPHERECAST_RADIUS = 1f;
    private const float SPHERECAST_DISTANCE_OFFSET = 3f;
    private const float MATERIAL_TWEEN_TIME = 0.25f;
    private CameraController cameraController;
    private Transform occludingObject;
    private Transform previousOccludingObject;
    private List<MaterialAlphaTween> tweeningMaterials;
    private List<MaterialAlphaTween> completedTweens;

    private List<GameObject> currentOcclusionGroup;
    private List<GameObject> newMarkedGroup;
    private bool markedForOcclusionUpdate;
    private Action UpdateTweenSet = delegate { };


    public ViewOcclusionManager(CameraController cameraController)
    {
        this.cameraController = cameraController;
        tweeningMaterials = new List<MaterialAlphaTween>();
        completedTweens = new List<MaterialAlphaTween>();
        currentOcclusionGroup = new List<GameObject>();
    }

    public void Update()
    {
        UpdateOccludingObjects();
        UpdateOcclusionGroup();
        UpdateTweeningMaterials();
    }

    public void UpdateOcclusionGroup()
    {
        if(markedForOcclusionUpdate)
        {
            markedForOcclusionUpdate = false;
            TweenUpOcclusionGroup(() =>
            {
                currentOcclusionGroup = newMarkedGroup;

                TweenDownOcclusionGroup(() =>
                {
                    newMarkedGroup = null;
                });
            });
        }
    }

    public void MarkOcclusionGroupForUpdate(List<GameObject> newGroup)
    {
        markedForOcclusionUpdate = true;
        newMarkedGroup = newGroup;
    }

    private void TweenUpOcclusionGroup(Action OnComplete)
    {
        if (currentOcclusionGroup.Count > 0)
        {
            int completedTweensCount = 0;
            foreach (GameObject occlusionGroupObject in currentOcclusionGroup)
            {
                UpdateTweenSet += () => AddTweeningMaterial(occlusionGroupObject.transform, 1f, MATERIAL_TWEEN_TIME, () =>
                {
                    completedTweensCount++;
                    if(completedTweensCount >= currentOcclusionGroup.Count)
                    {
                        OnComplete();
                    }
                });
            }
        }
        else
        {
            OnComplete();
        }
    }

    private void TweenDownOcclusionGroup(Action OnComplete)
    {
        if (currentOcclusionGroup.Count > 0)
        {
            int completedTweensCount = 0;
            foreach (GameObject occlusionGroupObject in currentOcclusionGroup)
            {
                UpdateTweenSet += () => AddTweeningMaterial(occlusionGroupObject.transform, 0f, MATERIAL_TWEEN_TIME, () =>
                {
                    completedTweensCount++;
                    if (completedTweensCount >= currentOcclusionGroup.Count)
                    {
                        OnComplete();
                    }
                });
            }
        }
        else
        {
            OnComplete();
        }
    }

    private void UpdateOccludingObjects()
    {
        Vector3 rayDirection = cameraController.Target.position - cameraController.MainCameraTransform.position;
        LayerMask layersToConsider = (1 << LayerMask.NameToLayer("OccludingEnvironment"));
        float distanceToPlayer = Vector3.Distance(cameraController.MainCameraTransform.position, cameraController.Target.position) - SPHERECAST_DISTANCE_OFFSET;
        RaycastHit[] allHits = Physics.SphereCastAll(cameraController.MainCameraTransform.position, SPHERECAST_RADIUS, rayDirection, distanceToPlayer, layersToConsider);
        Debug.DrawRay(cameraController.MainCameraTransform.position, rayDirection, Color.red, 0.1f);

        occludingObject = GetNearestHit(allHits);

        if (occludingObject != previousOccludingObject)
        {
            UpdateTweenSet += () => AddTweeningMaterial(occludingObject, 0.25f, MATERIAL_TWEEN_TIME);

            if (previousOccludingObject != null)
            {
                AddTweeningMaterial(previousOccludingObject, 1f, MATERIAL_TWEEN_TIME);
            }

            previousOccludingObject = occludingObject;
        }

    }

    private Transform GetNearestHit(RaycastHit[] allHits)
    {
        Transform nearestHit = null;
        float closestHitDistance = 0f;
        foreach(RaycastHit hit in allHits)
        {
            float currentDistance = Vector3.Distance(cameraController.MainCameraTransform.position, hit.point);
            if (closestHitDistance == 0 || currentDistance < closestHitDistance)
            {
                nearestHit = hit.transform;
                currentDistance = closestHitDistance;
            }
        }
        return nearestHit;
    }

    private void AddTweeningMaterial(Transform transform, float alphaTarget, float tweenTime, Action OnComplete = null)
    {
        if(transform != null)
        {
            MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();
            MaterialAlphaTween tween = new MaterialAlphaTween(meshRenderer, alphaTarget, meshRenderer.material.color.a, tweenTime);
            tween.OnComplete += () =>
            {
                completedTweens.Add(tween);
                OnComplete?.Invoke();
            };

            tweeningMaterials.Add(tween);
        }
    }

    private void UpdateTweeningMaterials()
    {
        foreach (MaterialAlphaTween tween in tweeningMaterials)
        {
            tween.Update();
        }

        UpdateTweenSet.Invoke();
        UpdateTweenSet = delegate { };

        tweeningMaterials = tweeningMaterials.Except(completedTweens).ToList();
        completedTweens.Clear();
    }
}
