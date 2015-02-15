//
// The component that manages the user's input for the game.
//

using System.Collections;
using UnityEngine;

public class GGInputComponent: MonoBehaviour {
	/* Managing state. */
	
	// Where the touch or click began as well as the input for the current frame in screen space.
	// Will be null if the user is not touching or clicking.
	private Vector2? inputOrigin;
	private Vector2? currentInput;
	
	/* Updating. */
	
	public void FixedUpdate() {
		if (Input.touchSupported) {
			this.UpdateTouchInput();
		}
		else {
			this.UpdateMouseInput();
		}
		
		this.UpdateArrow();
	}
	
	private void UpdateTouchInput() {
		// TODO
	}
	
	private void UpdateMouseInput() {
		if (Input.GetMouseButton(0)) {
			this.currentInput = Input.mousePosition;
			
			if (!this.inputOrigin.HasValue) {
				this.inputOrigin = this.currentInput;
			}
			
			var vector    = this.currentInput.Value - this.inputOrigin.Value;
			var threshold = GGInputComponent.inputThreshold;
			
			if (Mathf.Abs(vector.x) < threshold && Mathf.Abs(vector.y) < threshold) {
				this.currentInput = null;
			}
		}
		else {
			if (Input.GetMouseButtonUp(0)) {
				this.Shoot(Input.mousePosition);
			}
			
			this.inputOrigin  = null;
			this.currentInput = null;
		}
	}
	
	/* Updating the arrow. */
	
	private void UpdateArrow() {
		var arrow = GGGameSceneComponent.instance.arrow;
		
		if (this.inputOrigin.HasValue && this.currentInput.HasValue) {
			var camera         = Camera.main;
			var inputOrigin    = camera.ScreenToWorldPoint(this.inputOrigin.Value);
			var currentInput   = camera.ScreenToWorldPoint(this.currentInput.Value);
			inputOrigin.z      = 0.0f;
			currentInput.z     = 0.0f;
			var inputMagnitude = currentInput - inputOrigin;
			var arrowComponent = arrow.GetComponent<GGArrowComponent>();
			arrow.SetActive(true);
			arrowComponent.SetPosition(inputOrigin, inputOrigin - inputMagnitude);
		}
		else {
			arrow.SetActive(false);
		}
	}
	
	/* Shooting. */
	
	private void Shoot(Vector2 input) {
		Debug.Log("SHOOT");
	}
	
	/* Getting configuration values. */
	
	// The distance that the input must be dragged from the origin on either axis in screen space
	// (pixels) before it's considered a valid input.
	public const float inputThreshold = 4.0f;
}
