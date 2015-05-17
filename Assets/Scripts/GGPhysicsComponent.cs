//
// A component that manages some physics behavior, like configuring the physics materials used for
// the different surfaces in the game.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGPhysicsComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Awake() {
		this.grassMaterial      = new PhysicsMaterial2D();
		this.dirtMaterial       = new PhysicsMaterial2D();
		this.sandMaterial       = new PhysicsMaterial2D();
		this.rockMaterial       = new PhysicsMaterial2D();
		this.grassMaterial.name = "Grass";
		this.dirtMaterial.name  = "Dirt";
		this.sandMaterial.name  = "Sand";
		this.rockMaterial.name  = "Rock";
	}
	
	/* Accessing physics materials. */
	
	// The different physics materials used in the game, which are adjusted dynamically to get the
	// physics behavior that we want.
	public PhysicsMaterial2D grassMaterial { get; private set; }
	public PhysicsMaterial2D dirtMaterial  { get; private set; }
	public PhysicsMaterial2D sandMaterial  { get; private set; }
	public PhysicsMaterial2D rockMaterial  { get; private set; }
	
	/* Updating. */
	
	public void FixedUpdate() {
		var gameSceneComponent        = GGGameSceneComponent.instance;
		var mapComponent              = gameSceneComponent.mapComponent;
		var frictionMultiplier        = (Mathf.Abs(Vector2.Dot(Vector2.right, gameSceneComponent.ballRigidbody2D.velocity.normalized)) + 0.01f) * 3.0f;
		this.grassMaterial.friction   = GGPhysicsComponent.grassFriction * frictionMultiplier;
		this.grassMaterial.bounciness = GGPhysicsComponent.grassBounciness;
		this.dirtMaterial.friction    = GGPhysicsComponent.dirtFriction * frictionMultiplier;
		this.dirtMaterial.bounciness  = GGPhysicsComponent.dirtBounciness;
		this.sandMaterial.friction    = GGPhysicsComponent.sandFriction * frictionMultiplier;
		this.sandMaterial.bounciness  = GGPhysicsComponent.sandBounciness;
		this.rockMaterial.friction    = GGPhysicsComponent.rockFriction * frictionMultiplier;
		this.rockMaterial.bounciness  = GGPhysicsComponent.rockBounciness;
		this.UpdateCollider(mapComponent.groundCollider);
		
		foreach (var platformComponent in mapComponent.previousPlatformComponents) {
			foreach (var collider in platformComponent.colliders) {
				this.UpdateCollider(collider);
			}
		}
		
		foreach (var platformComponent in mapComponent.platformComponents) {
			foreach (var collider in platformComponent.colliders) {
				this.UpdateCollider(collider);
			}
		}
		
		foreach (var wallComponent in mapComponent.previousWallComponents) {
			this.UpdateCollider(wallComponent.GetComponent<Collider2D>());
		}
		
		foreach (var wallComponent in mapComponent.wallComponents) {
			this.UpdateCollider(wallComponent.GetComponent<Collider2D>());
		}
		
		this.UpdateCollider(mapComponent.groundCollider);
	}
	
	private void UpdateCollider(Collider2D collider) {
		collider.enabled = false;
		collider.enabled = true;
	}
	
	/* Getting configuration values. */
	
	// Physics material values.
	public const float grassFriction   = 0.4f;
	public const float grassBounciness = 0.275f;
	public const float dirtFriction    = 0.5f;
	public const float dirtBounciness  = 0.2f;
	public const float sandFriction    = 0.85f;
	public const float sandBounciness  = 0.1f;
	public const float rockFriction    = 0.1f;
	public const float rockBounciness  = 0.6f;
}
