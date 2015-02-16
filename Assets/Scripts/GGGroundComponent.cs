//
// The component that manages the ground object.
//

using System.Collections;
using UnityEngine;

public class GGGroundComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.renderer.material.color = GGGroundComponent.color;
	}
	
	/* Getting configuration values. */
	
	public static Color color = new Color(0.06f, 0.69f, 0.42f);
}
