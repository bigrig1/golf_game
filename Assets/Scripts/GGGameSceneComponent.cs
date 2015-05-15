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
		
		this.mapComponent     = this.GetComponent<GGMapComponent>();
		this.physicsComponent = this.GetComponent<GGPhysicsComponent>();
		this.sheepCount       = PlayerPrefs.GetInt("Sheep Count", 0);
		this.LoadGameObjects();
		this.ballComponent.LoadPersistedPosition();
		this.mapComponent.BuildFirstMap(PlayerPrefs.GetInt("Current Map Index", 0));
		
		switch (GGGameSceneComponent.mode) {
			case GGGameMode.Zen:     this.remainingStrokeCount = 0; break;
			case GGGameMode.Regular: this.remainingStrokeCount = 6; break;
			case GGGameMode.Hard:    this.remainingStrokeCount = 4; break;
		}
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
		Application.LoadLevel("Main Menu");
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
	
	/* Accessing game state. */
	
	// The number of strokes remaining. Only relevant in normal/hard modes.
	public int remainingStrokeCount = 0;
	
	// The number of sheep the player has collected.
	public int sheepCount { get; private set; }
	
	public void SheepWasHit(GGSheepComponent sheepComponent) {
		this.sheepCount += 1;
		PlayerPrefs.SetInt("Sheep Count", this.sheepCount);
		PlayerPrefs.SetInt("Sheep " + sheepComponent.id, 1);
	}
	
	/* Accessing the game mode. */
	
	// The current game mode. This is static because we want to be able to set it from the main menu
	// scene and have it persist when we load the game scene.
	public static GGGameMode mode = GGGameMode.Zen;
}
