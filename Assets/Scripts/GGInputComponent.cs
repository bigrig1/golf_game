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
	
	public void Update() {
		if (Input.touchSupported) {
			this.UpdateTouchInput();
		}
		else {
			this.UpdateMouseInput();
		}
	}
	
	public void FixedUpdate() {
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
			if (Input.GetMouseButtonUp(0) && this.inputOrigin.HasValue && this.currentInput.HasValue) {
				this.Shoot();
			}
			
			this.inputOrigin  = null;
			this.currentInput = null;
		}
	}
	
	/* Updating the arrow. */
	
	private void UpdateArrow() {
		var arrow = GGGameSceneComponent.instance.arrow;
		
		if (this.inputOrigin.HasValue && this.currentInput.HasValue) {
			var inputOrigin    = this.ConvertInputToWorldSpace(this.inputOrigin.Value);
			var currentInput   = this.ConvertInputToWorldSpace(this.currentInput.Value);
			var inputMagnitude = currentInput - inputOrigin;
			var arrowComponent = GGGameSceneComponent.instance.arrowComponent;
			arrow.SetActive(true);
			arrowComponent.SetPosition(inputOrigin, inputOrigin - inputMagnitude);
		}
		else {
			arrow.SetActive(false);
		}
	}
	
	/* Shooting. */
	
	private void Shoot() {
		GGGameSceneComponent.instance.ShootBall(this.ConvertInputToWorldSpace(this.inputOrigin.Value) - this.ConvertInputToWorldSpace(this.currentInput.Value));
	}
	
	/* Helpers. */
	
	private Vector2 ConvertInputToWorldSpace(Vector2 input) {
		return Camera.main.ScreenToWorldPoint(input);
	}
	
	/* Getting configuration values. */
	
	// The distance that the input must be dragged from the origin on either axis in screen space
	// (pixels) before it's considered a valid input.
	public const float inputThreshold = 4.0f;
}
