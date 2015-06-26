//
// The component that manages the game's background.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGBackgroundComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.CreateClouds();
		this.CreateBird();
	}
	
	/* Managing clouds. */
	
	private List<GameObject> clouds = new List<GameObject>();
	public float baseCloudVelocity = 0.0f;
	
	private void CreateClouds() {
		var cloudCount         = (int)Random.Range(5.0f, 8.0f);
		var cloudSectionHeight = GGMapComponent.usableScreenHeight / (float)cloudCount;
		var halfMapWidth       = GGMapComponent.mapWidth / 2.0f;
		this.baseCloudVelocity = Random.Range(0.75f, 1.45f);
		
		if (Random.value > 0.5f) {
			this.baseCloudVelocity *= -1.0f;
		}
		
		for (var i = 0; i < cloudCount; i += 1) {
			var cloud                   = GameObject.Instantiate(Resources.Load("Prefabs/Cloud")) as GameObject;
			var cloudComponent          = cloud.GetComponent<GGCloudComponent>();
			var baseY                   = (float)i * cloudSectionHeight;
			var scale                   = Random.Range(0.45f, 1.0f);
			var isFlipped               = false; //Random.value > 0.5f; TODO: Might look weird.
			cloud.transform.localScale  = new Vector3(isFlipped ? -scale : scale, scale, 1.0f);
			cloudComponent.velocity     = this.baseCloudVelocity * Random.Range(0.3f, 1.0f);
			cloudComponent.scrollFactor = Random.Range(0.16f, 0.27f);
			
			cloudComponent.position = new Vector2(
				Random.Range(-halfMapWidth - 9.0f, halfMapWidth + 9.0f),
				Random.Range(baseY, baseY + cloudSectionHeight - 1.5f)
			);
			
			this.clouds.Add(cloud);
		}
	}
	
	/* Managing birbs. */
	
	private GameObject bird;
	private Rigidbody2D birdRigidbody;
	private float timeToBird;
	private float birdDirection = 0.0f;
	
	private void CreateBird() {
		this.bird          = GameObject.Instantiate(Resources.Load("Prefabs/Bird")) as GameObject;
		this.birdRigidbody = this.bird.GetComponent<Rigidbody2D>();
		this.ResetBird();
	}
	
	private void ResetBird() {
		this.timeToBird    = Random.Range(15.0f, 60.0f);
		this.birdDirection = 0.0f;
		this.bird.SetActive(false);
	}
	
	private void LaunchBird() {
		this.bird.SetActive(true);
		var scroll             = 0.0f;
		var gameSceneComponent = GGGameSceneComponent.instance;
		
		if (gameSceneComponent != null && gameSceneComponent.cameraComponent != null) {
			scroll = gameSceneComponent.cameraComponent.scroll;
		}
		
		var baseScale                  = 0.4f;
		this.birdDirection             = Random.value > 0.5f ? 1.0f : -1.0f;
		this.birdRigidbody.velocity    = new Vector2(Random.Range(2.0f, 3.75f) * this.birdDirection, 0.0f);
		this.bird.transform.position   = new Vector3(21.0f * -this.birdDirection, scroll + GGMapComponent.usableScreenHeight * Random.Range(0.15f, 0.85f) + 1.0f, 0.0f);
		this.bird.transform.localScale = new Vector3(baseScale * -this.birdDirection, baseScale, baseScale);
	}
	
	/* Updating. */
	
	public void FixedUpdate() {
		var scroll             = 0.0f;
		var gameSceneComponent = GGGameSceneComponent.instance;
		
		if (gameSceneComponent != null && gameSceneComponent.cameraComponent != null) {
			scroll = gameSceneComponent.cameraComponent.scroll;
		}
		
		for (var i = 0; i < this.clouds.Count; i += 1) {
			var cloud          = this.clouds[i];
			var cloudComponent = cloud.GetComponent<GGCloudComponent>();
			var velocity       = cloudComponent.velocity;
			var x              = cloudComponent.position.x;
			var xLimit         = 21.0f;
			
			if ((velocity < 0.0f && x < -xLimit) || (velocity >= 0.0f && x > xLimit)) {
				var cloudSectionHeight  = GGMapComponent.usableScreenHeight / (float)this.clouds.Count;
				var position            = cloudComponent.position;
				var baseY               = (float)i * cloudSectionHeight;
				position.x              = -x * Random.Range(1.0f, 1.1f);
				position.y              = Random.Range(baseY, baseY + cloudSectionHeight - 1.5f);
				cloudComponent.position = position;
				cloudComponent.velocity = this.baseCloudVelocity * Random.Range(0.3f, 1.0f);
			}
			
			if (cloudComponent.transform.position.y < scroll - 1.5f) {
				cloudComponent.yOffset       = scroll + GGMapComponent.usableScreenHeight + Random.Range(0.5f, 2.0f);
				cloudComponent.yOffsetScroll = scroll;
			}
		}
		
		this.timeToBird -= Time.deltaTime;
		
		if (this.timeToBird <= 0.0f && this.birdDirection == 0.0f) {
			this.LaunchBird();
		}
		
		if (this.birdDirection < 0.0f && this.bird.transform.position.x < -21.0f) {
			this.ResetBird();
		}
		else if (this.birdDirection > 0.0f && this.bird.transform.position.x > 21.0f) {
			this.ResetBird();
		}
	}
}
