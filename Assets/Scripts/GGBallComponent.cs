//
// The component that manages the ball.
//

using System.Collections;
using UnityEngine;

public class GGBallComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.renderer.material.color = GGBallComponent.color;
		this.rigidbody2D             = this.GetComponent<Rigidbody2D>();
	}
	
	/* Accessing components. */
	
	new private Rigidbody2D rigidbody2D;
	
	/* Updating. */
	
	private float durationUnderForceSleepThreshold = 0.0f;
	
	public void FixedUpdate() {
		if (this.rigidbody2D.IsAwake()) {
			if (this.rigidbody2D.velocity.magnitude < 0.05f) {
				this.durationUnderForceSleepThreshold += Time.deltaTime;
				
				if (this.durationUnderForceSleepThreshold > 0.2f) {
					this.rigidbody2D.Sleep();
				}
			}
			else {
				this.durationUnderForceSleepThreshold = 0.0f;
			}
		}
		else {
			this.durationUnderForceSleepThreshold = 0.0f;
		}
	}
	
	/* Getting configuration values. */
	
	public static Color color = new Color(1.0f, 0.92f, 0.81f);
}
