//
// The component that manages an individual cloud.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGCloudComponent: MonoBehaviour {
	/* Configuring the cloud. */
	
	public float velocity;
	public Vector2 position;
	public float yOffset;
	public float yOffsetScroll;
	public float scrollFactor;
	
	/* Updating. */
	
	public void Update() {
		var cameraScroll   = GGGameSceneComponent.instance.cameraComponent.scroll;
		var relativeScroll = cameraScroll - this.yOffsetScroll;
		this.position.x   += velocity * Time.deltaTime * (1.0f - this.scrollFactor);
		
		this.transform.position = new Vector3(
			this.position.x,
			this.position.y + relativeScroll * (1.0f - this.scrollFactor) + this.yOffset,
			0.0f	
		);
	}
}
