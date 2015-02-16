//
// The component that manages platform objects.
//

using System.Collections;
using UnityEngine;

public class GGPlatformComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.renderer.material.color = GGPlatformComponent.color;
	}
	
	/* Getting configuration values. */
	
	public static Color color = new Color(0.06f, 0.69f, 0.42f);
}
