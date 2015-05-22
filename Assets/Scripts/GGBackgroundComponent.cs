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
	}
	
	/* Managing clouds. */
	
	private List<GameObject> clouds = new List<GameObject>();
	private float baseCloudVelocity = 0.0f;
	
	private void CreateClouds() {
		var cloudCount         = (int)Random.Range(5.0f, 8.0f);
		var cloudSectionHeight = GGMapComponent.screenHeight / (float)cloudCount;
		var halfMapWidth       = GGMapComponent.mapWidth / 2.0f;
		this.baseCloudVelocity = Random.Range(0.6f, 1.2f);
		
		if (Random.value > 0.5f) {
			this.baseCloudVelocity *= -1.0f;
		}
		
		for (var i = 0; i < cloudCount; i += 1) {
			var cloud                  = GameObject.Instantiate(Resources.Load("Prefabs/Cloud")) as GameObject;
			var cloudComponent         = cloud.GetComponent<GGCloudComponent>();
			var baseY                  = (float)i * cloudSectionHeight;
			var scale                  = Random.Range(0.45f, 1.0f);
			var isFlipped              = false; //Random.value > 0.5f; TODO: Might look weird.
			cloud.transform.localScale = new Vector3(isFlipped ? -scale : scale, scale, 1.0f);
			cloudComponent.velocity    = this.baseCloudVelocity * Random.Range(0.3f, 1.0f);
			
			cloud.transform.position = new Vector3(
				Random.Range(-halfMapWidth - 9.0f, halfMapWidth + 9.0f),
				Random.Range(baseY, baseY + cloudSectionHeight - 1.5f),
				0.0f
			);
			
			this.clouds.Add(cloud);
		}
	}
	
	/* Updating. */
	
	public void FixedUpdate() {
		for (var i = 0; i < this.clouds.Count; i += 1) {
			var cloud          = this.clouds[i];
			var cloudComponent = cloud.GetComponent<GGCloudComponent>();
			var velocity       = cloudComponent.velocity;
			var x              = cloud.transform.position.x;
			var limit          = 21.5f;
			
			if ((velocity < 0.0f && x < -limit) || (velocity >= 0.0f && x > limit)) {
				var cloudSectionHeight   = GGMapComponent.screenHeight / (float)this.clouds.Count;
				var position             = cloud.transform.position;
				var baseY                = (float)i * cloudSectionHeight;
				position.x               = -x * Random.Range(1.0f, 1.1f);
				position.y               = Random.Range(baseY, baseY + cloudSectionHeight - 1.5f);
				cloud.transform.position = position;
				cloudComponent.velocity  = this.baseCloudVelocity * Random.Range(0.3f, 1.0f);
			}
		}
	}
}
