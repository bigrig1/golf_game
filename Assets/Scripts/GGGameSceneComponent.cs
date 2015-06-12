//
// The component that manages the game scene and the overall game logic.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGGameSceneComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		// Uncomment to reset progress.
		// PlayerPrefs.DeleteAll();
		
		var mode = GGGameSceneComponent.mode;
		
		GGSaveData.InitializeSaveData(mode);
		
		if (mode != GGGameMode.Zen) {
			this.seed                 = GGSaveData.GetSeed();
			this.remainingStrokeCount = GGSaveData.GetRemainingStrokeCount(mode == GGGameMode.Hard ? 4 : 6);
			GGSaveData.SetSeed(this.seed);
		}
		
		this.mapComponent     = this.GetComponent<GGMapComponent>();
		this.physicsComponent = this.GetComponent<GGPhysicsComponent>();
		this.sheepCount       = GGSaveData.GetSheepCount();
		this.LoadGameObjects();
		this.ballComponent.LoadPersistedPosition();
		this.mapComponent.BuildFirstMap(GGSaveData.GetCurrentMapIndex());
	}
	
	private void LoadGameObjects() {
		var transform  = this.transform;
		var childCount = transform.childCount;
		
		for (var i = 0; i < childCount; i += 1) {
			var childTransform = transform.GetChild(i);
			var child          = childTransform.gameObject;
			var name           = child.name;
			
			switch (name) {
				case "Ball":         this.ball  = child;                             break;
				case "Arrow":        this.arrow = child;                             break;
				case "Platform":     this.mapComponent.LoadPlatformPrototype(child); break;
				case "Easy Walls":   this.LoadWallPrototypes(child, "easy");         break;
				case "Normal Walls": this.LoadWallPrototypes(child, "normal");       break;
				case "Hard Walls":   this.LoadWallPrototypes(child, "hard");         break;
				case "Ground":       this.mapComponent.LoadGround(child);            break;
			}
		}
		
		this.ballComponent   = this.ball.GetComponent<GGBallComponent>();
		this.ballRigidbody2D = this.ball.GetComponent<Rigidbody2D>();
		this.ballCollider    = this.ball.GetComponent<CircleCollider2D>();
		this.arrowComponent  = this.arrow.GetComponent<GGArrowComponent>();
		this.cameraComponent = Camera.main.GetComponent<GGCameraComponent>();
		this.inputComponent  = this.GetComponent<GGInputComponent>();
	}
	
	private void LoadWallPrototypes(GameObject container, string difficulty) {
		var transform  = container.transform;
		var childCount = transform.childCount;
		
		for (var i = 0; i < childCount; i += 1) {
			var child = transform.GetChild(i).gameObject;
			this.mapComponent.LoadWallPrototype(child, difficulty);
		}
	}
	
	/* Game overing. */
	
	public void GameOverMan() {
		// TODO: Do something interesting.
		GGSaveData.DeleteSaveData(GGGameSceneComponent.mode);
		GGFaderComponent.shouldFadeIn = true;
		GGFaderComponent.FadeOut("Main Menu");
	}
	
	/* Accessing the component. */
	
	public static GGGameSceneComponent instance { get {
		if (_instance == null) {
			_instance = GameObject.FindObjectOfType<GGGameSceneComponent>();
		}
		
		return _instance;
	} }
	
	private static GGGameSceneComponent _instance;
	
	/* Accessing game objects and components. */
	
	// The ball object.
	[HideInInspector]
	public GameObject ball;
	
	// The ball's ball component.
	public GGBallComponent ballComponent { get; private set; }
	
	// The ball's rigidbody component.
	public Rigidbody2D ballRigidbody2D { get; private set; }
	
	// The ball's collider.
	public CircleCollider2D ballCollider { get; private set; }
	
	// The arrow object.
	[HideInInspector]
	public GameObject arrow;
	
	// The arrow object's arrow component.
	public GGArrowComponent arrowComponent { get; private set; }
	
	// The map component.
	public GGMapComponent mapComponent { get; private set; }
	
	// The physics component.
	public GGPhysicsComponent physicsComponent { get; private set; }
	
	// The camera component.
	public GGCameraComponent cameraComponent { get; private set; }
	
	// The input component.
	public GGInputComponent inputComponent { get; private set; }
	
	/* Accessing game state. */
	
	// The game's base seed for the RNG.
	[HideInInspector]
	public int seed = 168403912;
	
	// The number of strokes remaining. Only relevant in normal/hard modes.
	[HideInInspector]
	public int remainingStrokeCount = 0;
	
	// The number of sheep the player has collected.
	public int sheepCount { get; private set; }
	
	public void SheepWasHit(GGSheepComponent sheepComponent) {
		this.sheepCount += 1;
		GGSaveData.SetSheepCount(this.sheepCount);
		GGSaveData.SetSheepHitFlag(sheepComponent.id);
	}
	
	/* Accessing the game mode. */
	
	// The current game mode. This is static because we want to be able to set it from the main menu
	// scene and have it persist when we load the game scene.
	public static GGGameMode mode = GGGameMode.Zen;
}
