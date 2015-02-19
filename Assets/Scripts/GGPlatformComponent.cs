//
// The component that manages platform objects.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGPlatformComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		var physicsComponent = GGGameSceneComponent.instance.physicsComponent;
		var transform        = this.transform;
		var childCount       = transform.childCount;
		
		for (var i = 0; i < childCount; i += 1) {
			var child      = transform.GetChild(i);
			var collider2D = child.collider2D;
			
			switch (child.name) {
				case "Grass": collider2D.sharedMaterial = physicsComponent.grassMaterial;     break;
				case "Dirt":  collider2D.sharedMaterial = physicsComponent.dirtMaterial;      break;
				case "Sand":  collider2D.sharedMaterial = physicsComponent.sandMaterial;      break;
				case "Rock":  collider2D.sharedMaterial = physicsComponent.rockMaterial;      break;
				default:      Debug.LogError("Unhandled platform child name: " + child.name); break;
			}
			
			this.colliders.Add(collider2D);
		}
	}
	
	/* Getting child objects and components. */
	
	// The colliders of each portion of the platform.
	[HideInInspector]
	public List<Collider2D> colliders = new List<Collider2D>();
}
