//
// The component that manages the ball.
//

using System.Collections;
using UnityEngine;

public class GGBallComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.rigidbody2D          = this.GetComponent<Rigidbody2D>();
		this.shotAudioSource      = this.transform.Find("Shot Audio").GetComponent<AudioSource>();
		this.collisionAudioSource = this.transform.Find("Collision Audio").GetComponent<AudioSource>();
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
	
	public float grassPitchVariation = 0.0f;
	public float rockPitchVariation  = 0.0f;
	public float dirtPitchVariation  = 0.0f;
	public float sandPitchVariation  = 0.0f;
	public float sheepPitchVariation = 0.0f;
	
	public AudioClip[] smallShotAudioClips;
	public AudioClip[] mediumShotAudioClips;
	public AudioClip[] bigShotAudioClips;
	
	public AudioClip[] grassAudioClips;
	public AudioClip[] rockAudioClips;
	public AudioClip[] dirtAudioClips;
	public AudioClip[] sandAudioClips;
	public AudioClip[] sheepAudioClips;
	
	/* Accessing components. */
	
	new private Rigidbody2D rigidbody2D;
	private AudioSource shotAudioSource;
	private AudioSource collisionAudioSource;
	
	/* Getting information about the ball. */
	
	public bool isInHole { get; private set; }
	
	/* Shooting the ball. */
	
	public void Shoot(Vector2 force) {
		var magnitude                         = force.magnitude;
		this.durationUnderForceSleepThreshold = 0.0f;
		this.rigidbody2D.isKinematic          = false;
		this.shotAudioSource.pitch            = 1.0f + Random.Range(-this.shotPitchVariation, this.shotPitchVariation);
		
		if (magnitude >= this.bigShotForce) {
			this.shotAudioSource.clip = this.GetRandomAudioClip(this.bigShotAudioClips);
		}
		else if (magnitude >= this.mediumShotForce) {
			this.shotAudioSource.clip = this.GetRandomAudioClip(this.mediumShotAudioClips);
		}
		else {
			this.shotAudioSource.clip = this.GetRandomAudioClip(this.smallShotAudioClips);
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
				var pitchVariation               = this.collisionPitchVariation;
				this.collisionAudioSource.volume = this.minCollisionVolume + Mathf.Clamp01((magnitude - this.minCollisionVolumeForce) / forceRange) * volumeRange;
				
				switch (colliderName) {
					case "Ground": this.collisionAudioSource.clip = this.GetRandomAudioClip(this.grassAudioClips); pitchVariation += this.grassPitchVariation; break;
					case "Wall":   this.collisionAudioSource.clip = this.GetRandomAudioClip(this.rockAudioClips);  pitchVariation += this.rockPitchVariation;  break;
					case "Dirt":   this.collisionAudioSource.clip = this.GetRandomAudioClip(this.dirtAudioClips);  pitchVariation += this.dirtPitchVariation;  break;
					case "Grass":  this.collisionAudioSource.clip = this.GetRandomAudioClip(this.grassAudioClips); pitchVariation += this.grassPitchVariation; break;
					case "Sand":   this.collisionAudioSource.clip = this.GetRandomAudioClip(this.sandAudioClips);  pitchVariation += this.sandPitchVariation;  break;
					case "Rock":   this.collisionAudioSource.clip = this.GetRandomAudioClip(this.rockAudioClips);  pitchVariation += this.rockPitchVariation;  break;
					case "Sheep":  this.collisionAudioSource.clip = this.GetRandomAudioClip(this.sheepAudioClips); pitchVariation += this.sheepPitchVariation; break;
					default:       Debug.LogError("Audio event encountered unhandled collider name " + colliderName + "."); break;
				}
				
				this.collisionAudioSource.pitch = 1.0f + Random.Range(-pitchVariation, pitchVariation);
				this.collisionAudioSource.Play();
			}
		}
	}
	
	public void OnTriggerEnter2D(Collider2D collider) {
		this.isInHole = true;
	}
	
	public void OnTriggerStay2D(Collider2D collider) {
		this.isInHole = true;
	}
	
	/* Getting audio clips. */
	
	private System.Random random = new System.Random();
	
	private AudioClip GetRandomAudioClip(AudioClip[] clips) {
		return clips[random.Next(clips.Length)];
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
		
		this.isInHole = false;
	}
}
