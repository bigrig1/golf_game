//
// The component that manages the game's camera.
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGCameraComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Awake() {
		// Nothing yet.
	}
	
	/* Updaing. */
	
	private bool poop = false;
	private bool poop2 = false;
	
	public void Update() {
		this.transform.Translate(new Vector3(0.0f, 1.75f * Time.deltaTime, 0.0f));
		
		if (!poop && this.transform.position.y >= 23.0f) {
			poop = true;
			Debug.Log("YA");
			GGGameSceneComponent.instance.mapComponent.BuildNextMap();
		}
		else if (!poop2 && this.transform.position.y >= 28.0f) {
			poop2 = true;
			Debug.Log("YA 2");
			GGGameSceneComponent.instance.mapComponent.BuildNextMap();
		}
	}
	
	/* Getting configuration values. */
	
	// TODO
}
