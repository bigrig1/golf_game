//
// The component that manages a sheep.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGSheepComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Awake() {
		this.rigidbody2D = this.GetComponent<Rigidbody2D>();
		this.audioSource = this.transform.Find("Audio").GetComponent<AudioSource>();
	}
	
	/* Configuring the sheep. */
	
	// Whether or not this is a hanging sheep.
	[HideInInspector]
	public bool isHanging = false;
	
	[HideInInspector]
	public bool managesVelocity = true;
	
	// The sheep's ID.
	[HideInInspector]
	public string id = "";
	
	public float collisionPitchVariation = 0.1f;
	public AudioClip[] collisionAudioClips;
	
	/* Getting components. */
	
	[HideInInspector]
	new public Rigidbody2D rigidbody2D;
	private AudioSource audioSource;
	
	/* Responding to collisions. */
	
	public void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.GetComponent<GGBallComponent>() != null) {
			this.MakeFall(false);
			var impulse = new Vector2(-collision.relativeVelocity.x, Mathf.Max(0.0f, -collision.relativeVelocity.y)) * 0.45f;
			this.rigidbody2D.AddForce(impulse, ForceMode2D.Impulse);
			GGGameSceneComponent.instance.SheepWasHit(this);
			
			this.audioSource.pitch = 1.0f + Random.Range(-this.collisionPitchVariation, this.collisionPitchVariation);
			this.audioSource.clip  = this.collisionAudioClips[random.Next(this.collisionAudioClips.Length)];
			this.audioSource.Play();
		}
	}
	
	private System.Random random = new System.Random();
	
	/* Making sheep fall. */
	
	public void MakeFall(bool parachuteImmediately) {
		this.rigidbody2D.isKinematic = false;
		this.isFalling               = true;
		this.fallTime                = 0.0f;
		this.gameObject.layer        = 0;
		
		if (parachuteImmediately) {
			this.DeployParachute();
		}
	}
	
	private void DeployParachute() {
		this.isFalling                = false;
		this.isParachuting            = true;
		this.rigidbody2D.velocity    *= 0.75f;
		this.rigidbody2D.gravityScale = 0.0f;
	}
	
	/* Updating. */
	
	private bool isFalling     = false;
	private bool isParachuting = false;
	private float fallTime     = 0.0f;
	
	public void FixedUpdate() {
		if (this.isFalling) {
			this.fallTime += Time.deltaTime;
			
			if (this.fallTime > 0.7f) {
				this.DeployParachute();
			}
		}
		
		if (this.isParachuting && this.managesVelocity) {
			var velocity              = this.rigidbody2D.velocity;
			velocity.x               *= 0.985f;
			velocity.y                = Mathf.MoveTowards(velocity.y, Mathf.Min(-2.5f, velocity.y), 0.1f);
			this.rigidbody2D.velocity = velocity;
		}
		
		var animator = this.GetComponent<Animator>();
		animator.SetBool("Is Hanging",     this.isHanging);
		animator.SetBool("Is Falling",     this.isFalling);
		animator.SetBool("Is Parachuting", this.isParachuting);
		
		if (this.isFalling || this.isParachuting) {
			var screenPosition = Camera.main.WorldToViewportPoint(transform.position);
			
			if (screenPosition.y < -0.2f) {
				GameObject.Destroy(this.gameObject);
			}
		}
	}
}
