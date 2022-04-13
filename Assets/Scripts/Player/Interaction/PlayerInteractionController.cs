using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionController
{
	public Action<InteractionPoint> OnInteractionStarted = delegate { };
	public Action OnInteractionAvailable = delegate { };
	public Action OnAvailableInteractionLost = delegate { };
	private Animator animator;
	private PlayerObjectData playerObjectData;
    private AnimationListener animationListener;
    private InteractionPoint availableInteractionPoint;
	private PlayerCollision playerCollision;
	private PlayerInteractionBehaviourFactory behaviourFactory;
	private Dictionary<InteractionPoint, InteractionBehaviour> behaviours;
	private Transform playerTransform;

	public PlayerInteractionController(Transform levelObjects, Transform playerTransform, Animator animator, 
		PlayerCollision collision, PickupEvents pickupEvents, PlayerObjectData playerObjectData)
    {
        this.animator = animator;
		this.playerCollision = collision;
		this.playerObjectData = playerObjectData;
		this.playerTransform = playerTransform;

		behaviours = new Dictionary<InteractionPoint, InteractionBehaviour>();
		behaviourFactory = new PlayerInteractionBehaviourFactory(pickupEvents);

		Transform interactionPointsRoot = levelObjects.Find("Interaction");
        if(interactionPointsRoot != null)
        {
            InitialiseInteractionPoints(interactionPointsRoot);
        }
	}

    private void InitialiseInteractionPoints(Transform interactionPointsRoot)
    {
		InteractionPoint[] interactionPoints = interactionPointsRoot.GetComponentsInChildren<InteractionPoint>();

		foreach (InteractionPoint interactionPoint in interactionPoints)
        {
			InteractionBehaviour behaviour = behaviourFactory.Create(interactionPoint);

			interactionPoint.collisionCallbacks.OnTriggerStayed += (other) =>
			{
				CheckInteraction(interactionPoint, behaviour, other);
			};
			interactionPoint.collisionCallbacks.OnTriggerExited += (other) => OnInteractionPointExit(interactionPoint);
		}
    }

	private void CheckInteraction(InteractionPoint interactionPoint, InteractionBehaviour behaviour, Collider collider)
	{
		if (playerCollision.OwnsCollider(collider))
		{
			Vector3 targetPositionFloor = new Vector3(interactionPoint.transform.position.x, 0f, interactionPoint.transform.position.z);
			Vector3 playerPositionFloor = new Vector3(collider.transform.position.x, 0f, collider.transform.position.z);

			Vector3 direction = targetPositionFloor - playerPositionFloor;
			float angle = Vector3.Angle(direction, collider.transform.forward);

			if (Mathf.Abs(angle) <= 60f && behaviour.CanInteract())
			{
				if(availableInteractionPoint != interactionPoint)
				{
					availableInteractionPoint = interactionPoint;
					OnInteractionAvailable();
				}
			}
			else
			{
				if (availableInteractionPoint != null && availableInteractionPoint == interactionPoint)
				{
					availableInteractionPoint = null;
					OnAvailableInteractionLost();
				}
			}
		}
	}

	public void Update()
	{
		//if(availableInteractionPoint != null)
		//{
		//	playerObjectData.LeftHandIKChain.weight = ConvertDistanceToIKChainWeight();
		//	if(playerObjectData.LeftHandIKChain.weight >= IKWeightThresholdBeforeInteraction)
		//	{
				
		//	}
		//	else
		//	{
				
		//	}
		//}
	}

	private float ConvertDistanceToIKChainWeight()
	{
		Vector3 playerPositionFloor = new Vector3(playerTransform.position.x, 0f, playerTransform.position.z);
		Vector3 interactionOffsetFloor = new Vector3(availableInteractionPoint.playerInteractionOffset.position.x, 0f, availableInteractionPoint.playerInteractionOffset.position.z);
		float weight = 1 - Vector3.Distance(playerPositionFloor, interactionOffsetFloor) + 0.5f;
		return weight;
	}

	private void OnInteractionPointExit(InteractionPoint interactionPoint)
	{
		if (availableInteractionPoint != null && availableInteractionPoint == interactionPoint)
		{
			availableInteractionPoint = null;
		}
	}
	public bool CanInteract()
	{
		return availableInteractionPoint != null;
	}

	public void Interact()
	{
		OnInteractionStarted(availableInteractionPoint);
	}
}
