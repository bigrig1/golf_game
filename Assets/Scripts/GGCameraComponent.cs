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
	
	public void Update() {
		this.transform.Translate(new Vector3(0.0f, 0.25f * Time.deltaTime, 0.0f));
	}
	
	/* Getting configuration values. */
	
	// TODO
}
