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
	
	// Whether or not the ball has been recently hit. This will be set to true when the player hits
	// the ball and back to false once it comes to a rest.
	private bool ballHasBeenHit = false;
	
	// Whether or not debug input is enabled. Should disable this before doing builds.
	private bool debugInputIsEnabled = true;
	
	/* Updating. */
	
	public void Update() {
		if (GGGameSceneComponent.instance.cameraComponent.isMovingToNextMap) {
			this.inputOrigin  = null;
			this.currentInput = null;
		}
		else if (Input.touchSupported) {
			this.UpdateTouchInput();
		}
		else {
			this.UpdateMouseInput();
		}
	}
	
	public void FixedUpdate() {
		this.UpdateArrow();
		
		var gameSceneComponent = GGGameSceneComponent.instance;
		
		if (this.ballHasBeenHit && gameSceneComponent.ballRigidbody2D.isKinematic) {
			this.ballHasBeenHit = false;
			
			if (gameSceneComponent.ballComponent.isInHole) {
				gameSceneComponent.ballComponent.containingHole.GetComponent<Collider2D>().enabled = false;
				gameSceneComponent.mapComponent.BuildNextMap();
				gameSceneComponent.cameraComponent.MoveToNextMap();
			}
		}
	}
	
	private void UpdateTouchInput() {
		if (Input.touchCount > 0) {
			var touch = Input.GetTouch(0);
			
			if (touch.phase == TouchPhase.Ended) {
				if (this.inputOrigin.HasValue && this.currentInput.HasValue) {
					this.Shoot();
				}
				
				this.inputOrigin  = null;
				this.currentInput = null;
			}
			else {
				this.currentInput = this.ConvertInputToWorldSpace(touch.position);
				
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
		}
		else {
			this.inputOrigin  = null;
			this.currentInput = null;
		}
	}
	
	private void UpdateMouseInput() {
		if (this.debugInputIsEnabled && Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0)) {
			var ball                = GGGameSceneComponent.instance.ball;
			ball.transform.position = this.ConvertInputToWorldSpace(Input.mousePosition);
		}
		else if (Input.GetMouseButton(0)) {
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
			var gameSceneComponent = GGGameSceneComponent.instance;
			var cameraComponent    = gameSceneComponent.cameraComponent;
			var arrowComponent     = gameSceneComponent.arrowComponent;
			var inputOrigin        = this.inputOrigin.Value;
			var currentInput       = this.currentInput.Value;
			var inputVector        = currentInput - inputOrigin;
			var ballOrigin         = (Vector2)gameSceneComponent.ball.transform.position;
			arrowComponent.power   = inputVector.magnitude / GGInputComponent.maxInputThreshold;
			arrowComponent.isFaded = this.ballHasBeenHit || cameraComponent.isMovingToNextMap;
			arrowComponent.SetPosition(ballOrigin, ballOrigin - inputVector);
			arrow.SetActive(true);
		}
		else {
			arrow.SetActive(false);
		}
	}
	
	/* Shooting. */
	
	private void Shoot() {
		if (!this.ballHasBeenHit) {
			var ballComponent = GGGameSceneComponent.instance.ballComponent;
			ballComponent.Shoot((this.inputOrigin.Value - this.currentInput.Value) * GGInputComponent.inputForce);
			this.ballHasBeenHit = true;
		}
	}
	
	/* Helpers. */
	
	private Vector2 ConvertInputToWorldSpace(Vector2 input) {
		return Camera.main.ScreenToWorldPoint(input);
	}
	
	/* Getting configuration values. */
	
	// The minimum and maximum magnitudes of the input vector in screen space (pixels).
	public const float minInputThreshold = 0.85f;
	public const float maxInputThreshold = 9.0f;
	
	// A multiplier that gets applied to the input vector to determine the amount of force to use
	// when shooting the ball.
	public const float inputForce = 2.5f;
}
