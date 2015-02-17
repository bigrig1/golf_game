//
// The component that manages platform objects.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGPlatformComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		var transform  = this.transform;
		var childCount = transform.childCount;
		
		for (var i = 0; i < childCount; i += 1) {
			this.colliders.Add(transform.GetChild(i).collider2D);
		}
	}
	
	/* Getting child objects and components. */
	
	// The colliders of each portion of the platform.
	[HideInInspector]
	public List<Collider2D> colliders = new List<Collider2D>();
}
