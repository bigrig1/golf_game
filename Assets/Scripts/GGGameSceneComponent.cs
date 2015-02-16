//
// The component that manages the game scene and the overall game logic.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGGameSceneComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Awake() {
		this.LoadGameObjects();
	}
	
	private void LoadGameObjects() {
		var transform  = this.transform;
		var childCount = transform.childCount;
		
		for (var i = 0; i < childCount; i += 1) {
			var childTransform = transform.GetChild(i);
			var child          = childTransform.gameObject;
			var name           = child.name;
			
			switch (name) {
				case "Ball":     this.ball  = child;                                                     break;
				case "Arrow":    this.arrow = child;                                                     break;
				case "Platform": this.platformComponents.Add(child.GetComponent<GGPlatformComponent>()); break;
				case "Wall":     this.wallComponents.Add(child.GetComponent<GGWallComponent>());         break;
				case "Ground":   this.ground = child;                                                    break;
			}
		}
		
		this.ballRigidbody2D = this.ball.rigidbody2D;
		this.ballCollider    = this.ball.GetComponent<CircleCollider2D>();
		this.arrowComponent  = this.arrow.GetComponent<GGArrowComponent>();
		this.groundComponent = this.ground.GetComponent<GGGroundComponent>();
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
	
	// The arrow object.
	[HideInInspector]
	public GameObject arrow;
	
	// The ground object.
	[HideInInspector]
	public GameObject ground;
	
	// The ball's rigidbody component.
	public Rigidbody2D ballRigidbody2D { get; private set; }
	
	// The ball's collider.
	public CircleCollider2D ballCollider { get; private set; }
	
	// The arrow object's arrow component.
	public GGArrowComponent arrowComponent { get; private set; }
	
	// The ground component.
	public GGGroundComponent groundComponent { get; private set; }
	
	// The list of platform components that are currently being used.
	public List<GGPlatformComponent> platformComponents = new List<GGPlatformComponent>();
	
	// The list of wall components that are currently being used.
	public List<GGWallComponent> wallComponents = new List<GGWallComponent>();
	
	/* Accessing physics materials. */
	
	// The different physics materials used in the game, which are adjusted dynamically to get the
	// physics behavior that we want.
	public PhysicsMaterial2D ballMaterial;
	public PhysicsMaterial2D grassMaterial;
	public PhysicsMaterial2D rockMaterial;
	
	/* Shooting the ball. */
	
	public void ShootBall(Vector2 inputVector) {
		this.ballRigidbody2D.AddForce(inputVector * GGGameSceneComponent.inputForce, ForceMode2D.Impulse);
	}
	
	/* Updating. */
	
	public void FixedUpdate() {
		var frictionMultiplier        = (Mathf.Abs(Vector2.Dot(Vector2.right, this.ballRigidbody2D.velocity.normalized)) + 0.01f) * 2.0f;
		this.ballMaterial.friction    = GGGameSceneComponent.ballFriction;
		this.ballMaterial.bounciness  = GGGameSceneComponent.ballBounciness;
		this.grassMaterial.friction   = GGGameSceneComponent.grassFriction * frictionMultiplier;
		this.grassMaterial.bounciness = GGGameSceneComponent.grassBounciness;
		this.rockMaterial.friction    = GGGameSceneComponent.rockFriction * frictionMultiplier;
		this.rockMaterial.bounciness  = GGGameSceneComponent.rockBounciness;
		
		this.UpdateCollider(this.ballCollider);
		this.UpdateCollider(this.ground.collider2D);
		
		foreach (var platformComponent in this.platformComponents) {
			this.UpdateCollider(platformComponent.collider2D);
		}
		
		foreach (var wallComponent in this.wallComponents) {
			this.UpdateCollider(wallComponent.collider2D);
		}
	}
	
	private void UpdateCollider(Collider2D collider) {
		collider.enabled = false;
		collider.enabled = true;
	}
	
	/* Getting configuration values. */
	
	// A multiplier that gets applied to the input vector to determine the amount of force to use
	// when shooting the ball.
	public const float inputForce = 2.25f;
	
	// Physics material values.
	public const float ballFriction    = 0.25f;
	public const float ballBounciness  = 0.0f;
	public const float grassFriction   = 0.4f;
	public const float grassBounciness = 0.275f;
	public const float rockFriction    = 0.1f;
	public const float rockBounciness  = 0.6f;
}
