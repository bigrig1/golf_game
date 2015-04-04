//
// The component that manages a sheep.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGSheepComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.rigidbody2D = this.GetComponent<Rigidbody2D>();
	}
	
	/* Configuring the sheep. */
	
	// Whether or not this is a hanging sheep.
	public bool isHangingSheep = false;
	
	/* Getting components. */
	
	new public Rigidbody2D rigidbody2D;
	
	/* Responding to collisions. */
	
	public void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.GetComponent<GGBallComponent>() != null) {
			var impulse                  = new Vector2(-collision.relativeVelocity.x, Mathf.Max(0.0f, -collision.relativeVelocity.y)) * 0.45f;
			this.rigidbody2D.isKinematic = false;
			this.isFalling               = true;
			this.fallTime                = 0.0f;
			this.gameObject.layer        = 0;
			this.rigidbody2D.AddForce(impulse, ForceMode2D.Impulse);
			
			GGGameSceneComponent.instance.sheepCount += 1;
		}
	}
	
	/* Updating. */
	
	private bool isFalling     = false;
	private bool isParachuting = false;
	private float fallTime     = 0.0f;
	
	public void FixedUpdate() {
		if (this.isFalling) {
			this.fallTime += Time.deltaTime;
			
			if (this.fallTime > 0.7f) {
				this.isFalling                = false;
				this.isParachuting            = true;
				this.rigidbody2D.velocity    *= 0.75f;
				this.rigidbody2D.gravityScale = 0.0f;
			}
		}
		
		if (this.isParachuting) {
			var velocity              = this.rigidbody2D.velocity;
			velocity.x               *= 0.985f;
			velocity.y                = Mathf.MoveTowards(velocity.y, Mathf.Min(-2.5f, velocity.y), 0.1f);
			this.rigidbody2D.velocity = velocity;
		}
		
		var animator = this.GetComponent<Animator>();
		animator.SetBool("Is Hanging",     this.isHangingSheep);
		animator.SetBool("Is Falling",     this.isFalling);
		animator.SetBool("Is Parachuting", this.isParachuting);
		
		if (this.isFalling || this.isParachuting) {
			var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
			
			if (screenPosition.y < 150.0f) {
				Debug.Log("DONE");
			}
		}
	}
}