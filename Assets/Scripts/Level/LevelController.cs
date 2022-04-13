using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{
    public int levelID;
	public Action OnLoadComplete = delegate { };
	public Action<float> OnLevelComplete = delegate { };
	public Action<Levels.Data> OnLevelLoadRequest = delegate { };
	public Action OnExitRequest = delegate { };
	public Action<ItemType> OnItemConsumed = delegate { };
    public Action OnGuardSpawned = delegate { };
    public Action OnGuardKilled = delegate { };
    public Action OnPickupCollected = delegate { };
	public Action<int> OnGuardsSetup = delegate { };	//S3 - Assignment 02

    public CameraController cameraController;
    private UIController uiController;
	private GuardManager guardManager;
    private Player player;
	private PickupController pickupController;
	private InventoryController inventory;
	private AudioManager audioManager;
	private LevelStatsController levelStatsController;
	private LevelSFXController sfxController;
	private ProjectileManager projectileManager;
	private LevelEndCutscene levelEndCutscene;

    public void Start()
    {
		LevelDependancies dependancies = GetComponentInChildren<LevelDependancies>();
		if(dependancies == null)
		{
			Debug.LogError("Unable to find LevelDependancies. Cannot play level.");
		}

		TimeController timeController = new TimeController();

		GuardEvents guardEvents = new GuardEvents();

		GameObject playerObj = CreatePlayerObject(dependancies.player);

		PlayerSettings playerSettings = playerObj.GetComponent<PlayerSettingsHolder>().playerSettings;

		ProjectilePool projectilePool = new ProjectilePool(dependancies.projectileLibrary, new ProjectileFactory(playerSettings));
		projectileManager = new ProjectileManager(projectilePool);

		guardManager = new GuardManager(guardEvents, projectilePool);
		guardManager.OnGuardsSetup += OnGuardsSetup;	//S3 - Assignment 02

		PickupEvents pickupEvents = new PickupEvents();

		//EndLevelInteraction endLevel = new EndLevelInteraction();


		player = CreatePlayer(playerObj, playerSettings,
			guardManager, pickupEvents, projectilePool);

		pickupController = new PickupController(transform, player.Controller, pickupEvents,
			dependancies.pickupLibrary);
		
		pickupEvents.OnPickupCollected += (pickup) =>
		{
			player.Controller.OnPickupCollected(pickup);
			OnPickupCollected();
		};
		pickupEvents.OnRemainingPickupCountUpdated += (requiredPickups) =>
		{
			levelStatsController.OnRemainingPickupsUpdated(requiredPickups);
		};

		player.Controller.OnDeathSequenceStarted += () =>
		{
			guardManager.ForceIdle();
		};

		player.Controller.OnDeathSequenceCompleted += () =>
		{
			FailLevel();
		};

		guardEvents.AddDamageDealtListener((damage, damageLocation) => player.Controller.TakeDamage(damage, damageLocation));
		guardEvents.AddKilledListener((guard) =>
		{
			levelStatsController.OnEnemyKilled();
			OnGuardKilled();
		});
		guardEvents.AddSpawnedListener((guard, controller) => OnGuardSpawned());

		cameraController = new CameraController(dependancies.cameraContainer,
			transform.parent, player.Controller.Transform);

		levelEndCutscene = new LevelEndCutscene(player, guardManager);//, endLevel);	//S3 - Assignment 04

		player.Interaction.OnInteractionStarted += (interaction) =>
		{
			if(interaction is EndLevelInteraction endLevel)
			{
				//EndLevel(endLevel);
				//endLevel.playableDirector.Play();
				levelEndCutscene.PlayComplete(endLevel, CompleteLevel);	//S3 - Assignment 04
				//CompleteLevel();
			}
		};

		levelStatsController = new LevelStatsController(guardManager, pickupController);

		audioManager = new AudioManager(transform);


		HUDController hudController = cameraController.MainCameraTransform.Find("HUDCanvas/HUD").GetComponent<HUDController>();
		player.Controller.OnPlayerDamageTaken += (currentHealth) => hudController.UpdatePlayerHealth(currentHealth);

		uiController = new UIController(player, cameraController.MainCameraTransform,
			timeController, levelID, inventory, pickupController, 
			pickupEvents, guardEvents, levelStatsController,
			dependancies.inventoryUI, dependancies.enemyHealthUI);

		uiController.OnLevelLoadRequest += (levelID) => OnLevelLoadRequest(levelID);
		uiController.OnExitRequested += () => OnExitRequest();

		sfxController = new LevelSFXController(player.Controller, player.Interaction,
			audioManager, pickupEvents, player.Equipment,
			guardEvents);

		_ = new LevelVFXController(dependancies.vfxLibrary, player.Controller, 
			guardEvents, guardManager);

		guardManager.OnLevelLoaded(transform);

		OnLoadComplete();
	}

	// private void EndLevel(EndLevelInteraction endLevel)
	// {
	// 	player.Broadcaster.EnableActions(ControlType.None);
	// 	guardManager.ForceIdle();
	// 	// endLevel.playableDirector.Play();				//Play the Timeline
	// 	// player.Controller.Face(endLevel.faceTarget.position, () =>
	// 	// {
	// 	// 	player.Animator.SetTrigger("ButtonPush");
	// 	// 	//StartCoroutine(LevelEndWait());
	// 	// });
	// }

	public IEnumerator LevelEndWait()
	{
		yield return new UnityEngine.WaitForSeconds(2);

		CompleteLevel();
	}

    public void Update()
    {
		levelStatsController.UpdateTime(Time.deltaTime);
		guardManager.Update();
		pickupController.Update();
		player.Controller.Update(cameraController.MainCameraTransform.forward);
		uiController.Update();
		audioManager.Update();
		player.Interaction.Update();
		cameraController.Update();
		projectileManager.Update();
		levelEndCutscene.Update();	//S3 - Assignment 05
	}

    public void OnDestroy()
	{
		player.Broadcaster.Destroy();
	}

	private GameObject CreatePlayerObject(GameObject playerObjectPrefab)
	{
		GameObject playerObject = GameObject.Instantiate(playerObjectPrefab, transform);
		playerObject.name = "Player";

		return playerObject;
	}

    private Player CreatePlayer(GameObject playerObject,
		PlayerSettings playerSettings, GuardManager guardManager, 
		PickupEvents pickupEvents, ProjectilePool projectilePool)
    {
        NavMeshAgent navMeshAgent = playerObject.GetComponent<NavMeshAgent>();

        Animator animator = playerObject.transform.Find("Human").GetComponent<Animator>();

        inventory = new InventoryController();
        PlayerObjectData objectData = playerObject.GetComponent<PlayerObjectData>();
		//TargetController targetController = playerObject.GetComponentInChildren<TargetController>();	//S3 - Assignment 04
		//Debug.Log(targetController);

		PlayerEvents events = new PlayerEvents();

		PlayerCollision collision = new PlayerCollision(playerObject.transform);

		GunTargetLocator gunTargetLocator = new GunTargetLocator(guardManager, playerSettings, objectData);
		PlayerInteractionController interaction = new PlayerInteractionController(transform, playerObject.transform, animator, collision, pickupEvents, objectData);

		PlayerInputBroadcaster broadcaster = new PlayerInputBroadcaster();

		PlayerEquipmentController equipment = new PlayerEquipmentController(playerObject.transform, inventory, objectData, 
			playerSettings, events, gunTargetLocator, broadcaster,
			projectilePool);
        equipment.OnItemConsumed += (item) => OnItemConsumed(item);

        PlayerController controller = new PlayerController(playerObject.transform, playerSettings, navMeshAgent, equipment,
			events, animator, interaction, collision, broadcaster);

		broadcaster.Callbacks.OnPlayerStartUseFired += () => controller.StartEquipmentUse();
		broadcaster.Callbacks.OnPlayerEndUseFired += () => controller.EndEquipmentUse();

		return new Player(controller, playerSettings, broadcaster, 
			equipment, events, animator, objectData,
			interaction);
	}

	private void FailLevel()
	{
		player.Broadcaster.EnableActions(ControlType.None);
		uiController.OnLevelFailed("You were killed!");
		sfxController.OnLevelFail();
	}

    private void CompleteLevel()
	{
		player.Broadcaster.EnableActions(ControlType.None);
        uiController.OnLevelComplete("Level Complete");
		sfxController.OnLevelComplete();
		OnLevelComplete(levelStatsController.LevelTime.Value);
		guardManager.ForceIdle();
	}
}

public class Player
{
    public PlayerController Controller { get; }
    public PlayerSettings Settings { get; }
    public PlayerInputBroadcaster Broadcaster { get; }
	public PlayerEquipmentController Equipment { get; }
	public PlayerEvents Events { get; }
	public Animator Animator { get; }
	public PlayerObjectData ObjectData { get; }
	public PlayerInteractionController Interaction { get; }

	public Player(PlayerController controller, PlayerSettings settings,
		PlayerInputBroadcaster broadcaster, PlayerEquipmentController equipment,
		PlayerEvents events, Animator animator, PlayerObjectData objectData,
		PlayerInteractionController interaction)
    {
        this.Controller = controller;
        this.Settings = settings;
        this.Broadcaster = broadcaster;
		this.Equipment = equipment;
		this.Events = events;
		this.Animator = animator;
		this.ObjectData = objectData;
		this.Interaction = interaction;
	}
}
