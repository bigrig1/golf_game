//
// The component that manages the ball.
//

using System.Collections;
using UnityEngine;

public class GGBallComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.renderer.material.color = GGBallComponent.color;
		this.rigidbody2D             = this.GetComponent<Rigidbody2D>();
		this.shotAudioSource         = this.transform.Find("Shot Audio").GetComponent<AudioSource>();
		this.collisionAudioSource    = this.transform.Find("Collision Audio").GetComponent<AudioSource>();
	}
	
	/* Configuring the component. */
	
	public float mediumShotForce    = 10.0f;
	public float bigShotForce       = 19.0f;
	public float shotPitchVariation = 0.1f;
	
	public float collisionTriggerForce   = 2.0f;
	public float minCollisionVolumeForce = 2.0f;
	public float maxCollisionVolumeForce = 10.0f;
	public float minCollisionVolume      = 0.1f;
	public float maxCollisionVolume      = 1.0f;
	public float collisionPitchVariation = 0.2f;
	
	public AudioClip smallShotAudioClip;
	public AudioClip mediumShotAudioClip;
	public AudioClip bigShotAudioClip;
	
	public AudioClip grassAudioClip;
	public AudioClip rockAudioClip;
	public AudioClip dirtAudioClip;
	public AudioClip sandAudioClip;
	
	/* Accessing components. */
	
	new private Rigidbody2D rigidbody2D;
	private AudioSource shotAudioSource;
	private AudioSource collisionAudioSource;
	
	/* Shooting the ball. */
	
	public void Shoot(Vector2 force) {
		var magnitude                         = force.magnitude;
		this.durationUnderForceSleepThreshold = 0.0f;
		this.rigidbody2D.isKinematic          = false;
		this.shotAudioSource.pitch            = 1.0f + Random.Range(-this.shotPitchVariation, this.shotPitchVariation);
		
		if (magnitude >= this.bigShotForce) {
			this.shotAudioSource.clip = this.bigShotAudioClip;
		}
		else if (magnitude >= this.mediumShotForce) {
			this.shotAudioSource.clip = this.mediumShotAudioClip;
		}
		else {
			this.shotAudioSource.clip = this.smallShotAudioClip;
		}
		
		this.shotAudioSource.Play();
		this.shouldCancelNextCollisionAudioEvent = true;
		this.rigidbody2D.AddForce(force, ForceMode2D.Impulse);
	}
	
	/* Responding to events. */
	
	private bool shouldCancelNextCollisionAudioEvent = false;
	
	public void OnCollisionEnter2D(Collision2D collision) {
		if (collision.relativeVelocity.magnitude > this.collisionTriggerForce) {
			if (this.shouldCancelNextCollisionAudioEvent) {
				this.shouldCancelNextCollisionAudioEvent = false;
			}
			else {
				var colliderName                 = collision.collider.gameObject.name;
				var magnitude                    = collision.relativeVelocity.magnitude;
				var forceRange                   = this.maxCollisionVolumeForce - this.minCollisionVolumeForce;
				var volumeRange                  = this.maxCollisionVolume - this.minCollisionVolume;
				this.collisionAudioSource.volume = this.minCollisionVolume + Mathf.Clamp01((magnitude - this.minCollisionVolumeForce) / forceRange) * volumeRange;
				this.collisionAudioSource.pitch  = 1.0f + Random.Range(-this.collisionPitchVariation, this.collisionPitchVariation);
				
				switch (colliderName) {
					case "Ground": this.collisionAudioSource.clip = this.grassAudioClip; break;
					case "Wall":   this.collisionAudioSource.clip = this.rockAudioClip;  break;
					case "Dirt":   this.collisionAudioSource.clip = this.dirtAudioClip;  break;
					case "Grass":  this.collisionAudioSource.clip = this.grassAudioClip; break;
					case "Sand":   this.collisionAudioSource.clip = this.sandAudioClip;  break;
					case "Rock":   this.collisionAudioSource.clip = this.rockAudioClip;  break;
					default:       Debug.LogError("Audio event encountered unhandled collider name " + colliderName + "."); break;
				}
				
				this.collisionAudioSource.Play();
			}
		}
	}
	
	/* Updating. */
	
	private float durationUnderForceSleepThreshold = 0.0f;
	
	public void FixedUpdate() {
		if (!this.rigidbody2D.isKinematic) {
			if (this.rigidbody2D.velocity.magnitude < 0.15f) {
				this.durationUnderForceSleepThreshold += Time.deltaTime;
				
				if (this.durationUnderForceSleepThreshold > 0.4f) {
					this.rigidbody2D.isKinematic = true;
				}
			}
			else {
				this.durationUnderForceSleepThreshold = 0.0f;
			}
		}
	}
	
	/* Getting configuration values. */
	
	public static Color color = new Color(1.0f, 0.92f, 0.81f);
}
