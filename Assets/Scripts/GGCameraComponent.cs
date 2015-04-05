//
// The component that manages the game's camera.
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGCameraComponent: MonoBehaviour {
	/* Moving the camera. */
	
	public void MoveToNextMap() {
		this.moveStartTime = Time.time;
		this.moveStartY    = this.transform.position.y;
	}
	
	/* Updating. */
	
	private float moveStartTime = 0.0f;
	private float moveStartY    = 0.0f;
	
	public void Update() {
		if (this.moveStartTime > 0.0f) {
			var mapComponent        = GGGameSceneComponent.instance.mapComponent;
			var relativeMapIndex    = mapComponent.currentMapIndex - mapComponent.initialMapIndex;
			var targetY             = GGMapComponent.mapHeight * ((float)relativeMapIndex + 0.5f);
			var duration            = Time.time - this.moveStartTime;
			var progress            = Mathf.Clamp01(duration / GGCameraComponent.mapChangeDuration);
			var position            = this.transform.position;
			position.y              = Easer.Ease(EaserEase.InOutSine, this.moveStartY, targetY, progress);
			this.transform.position = position;
			
			if (progress >= 1.0f) {
				this.moveStartTime = 0.0f;
				this.moveStartY    = 0.0f;
			}
		}
	}
	
	/* Getting configuration values. */
	
	public const float mapChangeDuration = 1.5f;
}
