using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController
{
	public Action<float> OnPlayerDamageTaken = delegate { };
	public Action OnDeathSequenceStarted = delegate { };
	public Action OnDeathSequenceCompleted = delegate { };
	public Action<ItemType> OnEquipmentUsed = delegate { };
	public Action<Vector3, ItemType> OnEquipmentCollidedWithEnvironment = delegate { };
	public Action<Guard, Vector3, ItemType> OnEquipmentCollidedWithGuard = delegate { };
	public Action OnInteractionAvailable = delegate { };
	public Action OnAvailableInteractionLost = delegate { };
	private Transform transform;

	private PlayerHealth playerHealth;

	private NavMeshAgent navMeshAgent;
	private Animator animator;
	private Rigidbody rigidbody;
	private PlayerAnimatorController playerAnimatorController;

	private PlayerSettings playerSettings;
	private PlayerEquipmentController equipment;
	private PlayerObjectData playerObjectData;
	private TargetController targetController;	//S3 - Assignment 05
	private PlayerInteractionController interactionController;
	private AnimationListener animationListener;
	private Vector3 lastDamageLocation;
	private Tween currentTween;
	private PlayerMovementController movementController;
	private PlayerCollision collision;
	private PlayerInputBroadcaster inputBroadcaster;
	private PlayerViewRelativeMovement viewRelativeMovement;

	public PlayerController(Transform transform, PlayerSettings settings, 
		NavMeshAgent navMeshAgent, PlayerEquipmentController equipmentController, 
		PlayerEvents playerEvents, Animator animator, 
		PlayerInteractionController interactionController, PlayerCollision collision,
		PlayerInputBroadcaster inputBroadcaster)
    {
		this.transform = transform;
		this.playerSettings = settings;
		this.navMeshAgent = navMeshAgent;
		this.equipment = equipmentController;
		this.animator = animator;
		this.interactionController = interactionController;
		this.collision = collision;
		this.inputBroadcaster = inputBroadcaster;

		playerHealth = new PlayerHealth(settings.PlayerMaxHP);

		playerHealth.OnDamageTaken += (currentHealth) =>
		{
			TakeDamage();
			OnPlayerDamageTaken(currentHealth);
		};

		playerHealth.OnKilled += () =>
		{
			OnDeathSequenceStarted();
			HandlePlayerDeath();
		};

		playerEvents.OnShotTargetSet += (target, onComplete) =>
		{
			Face(target, onComplete);
		};

		equipmentController.OnEquipmentUsed += (type) => OnEquipmentUsed(type);
		equipmentController.OnCollidedWithGuard += (guard, collisionPos, type) => OnEquipmentCollidedWithGuard(guard, collisionPos, type);
		equipmentController.OnCollidedWithEnvironment += (collisionPos, type) => OnEquipmentCollidedWithEnvironment(collisionPos, type);

		rigidbody = transform.GetComponent<Rigidbody>();
		playerAnimatorController = transform.Find("Human").GetComponent<PlayerAnimatorController>();
		
		playerObjectData = transform.GetComponent<PlayerObjectData>();

		//targetController = playerObjectData.GetComponentInChildren<TargetController>();	//S3 - Assignment 05
		//Debug.Log(targetController);

		navMeshAgent.updateRotation = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

		playerAnimatorController.Setup(animator, rigidbody, playerSettings.MoveSpeedMultiplier);
		playerAnimatorController.Setup(animator, rigidbody, playerSettings.MoveSpeedMultiplier);
		SetupAnimationListener();
		playerAnimatorController.Setup(animator, rigidbody, settings.MoveSpeedMultiplier);

		movementController = new PlayerMovementController(transform, animator, settings);
		viewRelativeMovement = new PlayerViewRelativeMovement(movementController, inputBroadcaster.Callbacks);

		interactionController.OnInteractionAvailable += () => OnInteractionAvailable();
		interactionController.OnAvailableInteractionLost += () => OnAvailableInteractionLost();
	}

	public void TakeDamage(float damageAmount, Vector3 damageLocation)
	{
		lastDamageLocation = damageLocation;
		playerHealth.TakeDamage(damageAmount);
	}

	public void Update(Vector3 viewForward)
	{
		equipment.Update();
		if(currentTween != null)
		{
			currentTween.Update();
		}
		viewRelativeMovement.Update(viewForward);
		movementController.Update();
	}

	private void SetupAnimationListener()
	{
		animationListener = transform.Find("Human").GetComponent<AnimationListener>();
		animationListener.OnWeightedAnimationEvent += (argument, weight) =>
		{
			switch (argument)
			{
				case "DeathAnimationComplete":
					DeathAnimationComplete();
					break;
			}
		};
	}

	private void DeathAnimationComplete()
	{
		currentTween = new WaitForSeconds(2f);
		currentTween.OnComplete += () =>
		{
			currentTween = null;
			OnDeathSequenceCompleted();
		};
	}

	private bool IsPlayerFacing(Vector3 location)
	{
		float dot = Vector3.Dot(transform.forward, (location - transform.position).normalized);
		if(dot > 0.5f)
		{
			return true;
		}

		return false;
	}

	private void HandlePlayerDeath()
	{
		inputBroadcaster.EnableActions(ControlType.None);
		if(IsPlayerFacing(lastDamageLocation))
		{
			animator.SetTrigger("DeathFront");
		}
		else
		{
			animator.SetTrigger("DeathBack");
		}
	}

	private void TakeDamage()
	{
		if (IsPlayerFacing(lastDamageLocation))
		{
			animator.SetTrigger("HitFront");
		}
		else
		{
			animator.SetTrigger("HitBack");
		}
	}

	public void Face(Vector3 target, Action OnComplete)
	{
		movementController.Face(target - transform.position, () => OnComplete());
	}

	public void Warp(Vector3 position)
	{
        navMeshAgent.Warp(position);
	}

	public void OnPickupCollected(Pickup pickup)
	{
		equipment.OnPickupCollected(pickup);
	}

	public void StartEquipmentUse()
	{
		if(interactionController.CanInteract())
		{
			interactionController.Interact();
		}
		else
		{
			equipment.StartUse();
		}
	}

	public void EndEquipmentUse()
	{
		equipment.EndUse();
	}

	public bool OwnsCollider(Collider collider)
	{
		return collision.OwnsCollider(collider);
	}

	public Vector3 Position
    {
        get
		{
            return transform.position;
		}
    }

	public Transform Transform
	{
		get
		{
			return transform;
		}
	}

    public Vector3 Forward
    {
        get
        {
            return transform.forward;
        }
    }

	public PlayerObjectData Objects
	{
		get
		{
			return playerObjectData;
		}
	}
}
