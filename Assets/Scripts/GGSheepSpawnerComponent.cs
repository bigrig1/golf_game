//
// A component that spawns some falling sheep.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGSheepSpawnerComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.CreateSheep();
	}
	
	/* Managing sheep. */
	
	private List<GameObject> sheep = new List<GameObject>();
	private Vector2 baseVelocity   = new Vector2(0.0f, -3.5f);
	
	private void CreateSheep() {
		var sheepCount = 8;
		
		for (var i = 0; i < sheepCount; i += 1) {
			var sheep                      = GameObject.Instantiate(Resources.Load("Prefabs/Sheep")) as GameObject;
			var sheepComponent             = sheep.GetComponent<GGSheepComponent>();
			sheepComponent.managesVelocity = false;
			this.ResetSheep(sheep, (float)i * 13.0f);
			sheepComponent.MakeFall(true);
			this.sheep.Add(sheep);
		}
	}
	
	private void ResetSheep(GameObject sheep, float offset) {
		var rigidbody2D            = sheep.GetComponent<Rigidbody2D>();
		var scale                  = Random.Range(0.4f, 1.0f);
		var isFlipped              = Random.value > 0.5f;
		sheep.transform.localScale = new Vector3(isFlipped ? -scale : scale, scale, 1.0f);
		rigidbody2D.velocity       = this.baseVelocity * scale * Random.Range(0.85f, 1.0f);
		
		sheep.transform.position = new Vector3(
			-6.0f + 12.0f * Random.value,
			36.5f + offset + Random.value * 3.5f,
			2.0f - scale
		);
	}
	
	/* Updating. */
	
	public void FixedUpdate() {
		for (var i = 0; i < this.sheep.Count; i += 1) {
			var sheep = this.sheep[i];
			
			if (sheep.transform.position.y < -5.0f) {
				this.ResetSheep(sheep, Random.value * 25.0f);
			}
		}
	}
}
