//
// The component that manages the user's input for the game.
//

using System.Collections;
using UnityEngine;

public class GGInputComponent: MonoBehaviour {
	/* Managing state. */
	
	// Where the touch or click began as well as the input for the current frame in world space.
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
			this.currentInput = this.ConvertInputToWorldSpace(Input.mousePosition);
			
			if (!this.inputOrigin.HasValue) {
				this.inputOrigin = this.currentInput;
			}
			
			var inputVector = this.inputOrigin.Value - this.currentInput.Value;
			var magnitude   = inputVector.magnitude;
			
			if (magnitude < GGInputComponent.minInputThreshold) {
				this.currentInput = null;
			}
			else if (magnitude > GGInputComponent.maxInputThreshold) {
				this.currentInput = this.inputOrigin.Value - inputVector.normalized * GGInputComponent.maxInputThreshold;
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
			var inputOrigin    = this.inputOrigin.Value;
			var currentInput   = this.currentInput.Value;
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
		GGGameSceneComponent.instance.ShootBall(this.inputOrigin.Value - this.currentInput.Value);
	}
	
	/* Helpers. */
	
	private Vector2 ConvertInputToWorldSpace(Vector2 input) {
		return Camera.main.ScreenToWorldPoint(input);
	}
	
	/* Getting configuration values. */
	
	// The minimum and maximum magnitudes of the input vector in screen space (pixels).
	public const float minInputThreshold = 0.5f;
	public const float maxInputThreshold = 8.0f;
}
