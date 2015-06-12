//
// The component that manages the game's camera.
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGCameraComponent: MonoBehaviour {
	/* Getting information about the camera. */
	
	public bool isMovingToNextMap { get {
		return this.moveStartTime > 0.0f;
	} }
	
	public float scroll { get {
		return this.transform.position.y - GGMapComponent.usableScreenHeight / 2.0f;
	} }
	
	/* Moving the camera. */
	
	public void MoveToNextMap() {
		this.moveStartTime = Time.time;
		this.moveStartY    = this.transform.position.y;
	}
	
	/* Updating. */
	
	private float moveStartTime         = 0.0f;
	private float moveStartY            = 0.0f;
	private bool didFinishTransitioning = false;
	
	public void Update() {
		if (this.isMovingToNextMap) {
			var mapComponent        = GGGameSceneComponent.instance.mapComponent;
			var targetY             = mapComponent.yOffset - (GGMapComponent.usableScreenHeight - GGMapComponent.uiHeight) * 0.5f;
			var duration            = Time.time - this.moveStartTime;
			var progress            = Mathf.Clamp01(duration / GGCameraComponent.mapChangeDuration);
			var position            = this.transform.position;
			position.y              = Easer.Ease(EaserEase.InOutSine, this.moveStartY, targetY, progress);
			this.transform.position = position;
			
			if (progress >= 1.0f) {
				this.moveStartTime          = 0.0f;
				this.moveStartY             = 0.0f;
				this.didFinishTransitioning = true;
			}
		}
	}
	
	public void FixedUpdate() {
		if (this.didFinishTransitioning) {
			GGGameSceneComponent.instance.BroadcastMessage("DidFinishTransitioningToMap", null, SendMessageOptions.DontRequireReceiver);
			this.didFinishTransitioning = false;
		}
	}
	
	/* Getting configuration values. */
	
	public const float mapChangeDuration = 1.85f;
}
