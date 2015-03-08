//
// The component that manages the game scene and the overall game logic.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGGameSceneComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.mapComponent     = this.GetComponent<GGMapComponent>();
		this.physicsComponent = this.GetComponent<GGPhysicsComponent>();
		this.LoadGameObjects();
		// TODO: Load initial map index.
		this.mapComponent.BuildFirstMap(0);
	}
	
	private void LoadGameObjects() {
		var transform  = this.transform;
		var childCount = transform.childCount;
		
		for (var i = 0; i < childCount; i += 1) {
			var childTransform = transform.GetChild(i);
			var child          = childTransform.gameObject;
			var name           = child.name;
			
			switch (name) {
				case "Ball":     this.ball  = child;                             break;
				case "Arrow":    this.arrow = child;                             break;
				case "Platform": this.mapComponent.LoadPlatformPrototype(child); break;
				case "Wall":     this.mapComponent.LoadWallPrototype(child);     break;
				case "Ground":   this.mapComponent.LoadGround(child);            break;
			}
		}
		
		this.ballRigidbody2D = this.ball.rigidbody2D;
		this.ballCollider    = this.ball.GetComponent<CircleCollider2D>();
		this.arrowComponent  = this.arrow.GetComponent<GGArrowComponent>();
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
}
