//
// The component that manages the ball.
//

using System.Collections;
using UnityEngine;

public class GGBallComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.renderer.material.color = GGBallComponent.color;
	}
	
	/* Getting configuration values. */
	
	public static Color color = new Color(1.0f, 0.92f, 0.81f);
}
