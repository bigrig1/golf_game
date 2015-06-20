//
// The component that manages the game's UI.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GGUIComponent: MonoBehaviour {
	/* Accessing UI components. */
	
	public Canvas canvas;
	public Text holeLabel;
	public Text sheepCountLabel;
	public Image strokeCountIcon;
	public Text strokeCountLabel;
	public Text powerLabel;
	public Text powerLabelShadow;
	public Button undoButton;
	
	/* Initializing. */
	
	public void Start() {
		var strokeCountIsEnabled = GGGameSceneComponent.mode != GGGameMode.Zen;
		strokeCountIcon.gameObject.SetActive(strokeCountIsEnabled);
		strokeCountLabel.gameObject.SetActive(strokeCountIsEnabled);
	}
	
	/* Updating. */
	
	public void FixedUpdate() {
		var gameSceneComponent  = GGGameSceneComponent.instance;
		var inputComponent      = gameSceneComponent.inputComponent;
		var ballComponent       = gameSceneComponent.ballComponent;
		var arrowComponent      = gameSceneComponent.arrowComponent;
		var sheepCountString    = "" + gameSceneComponent.sheepCount;
		var holeString          = "Hole " + (gameSceneComponent.mapComponent.currentMapIndex + 1);
		sheepCountLabel.text    = sheepCountString;
		holeLabel.text          = holeString;
		undoButton.interactable = !inputComponent.ballHasBeenHit && ballComponent.canRestoreUndoPosition;
		
		if (GGGameSceneComponent.mode != GGGameMode.Zen) {
			var strokeCountString = "" + gameSceneComponent.remainingStrokeCount;
			strokeCountLabel.text = strokeCountString;
		}
		
		if (inputComponent.inputOrigin.HasValue && inputComponent.currentInput.HasValue && !inputComponent.ballHasBeenHit) {
			var canvasTransform              = (RectTransform)this.canvas.transform;
			var labelTransform               = (RectTransform)this.powerLabel.transform;
			var shadowTransform              = (RectTransform)this.powerLabelShadow.transform;
			var screenPosition               = Camera.main.WorldToViewportPoint(ballComponent.transform.position);
			var screenSize                   = canvasTransform.rect.size;
			var position                     = new Vector2(screenPosition.x * screenSize.x + 1.0f, -(1.0f -screenPosition.y) * screenSize.y - 20.0f);
			var shadowPosition               = position;
			var power                        = arrowComponent.power;
			var percentage                   = (int)Mathf.Clamp(Mathf.Round(power * 100.0f), 0.0f, 100.0f);
			shadowPosition.y                -= 1.0f;
			labelTransform.anchoredPosition  = position;
			shadowTransform.anchoredPosition = shadowPosition;
			this.powerLabel.text             = percentage + "%";
			this.powerLabelShadow.text       = this.powerLabel.text;
			this.powerLabel.enabled          = true;
			this.powerLabelShadow.enabled    = true;
		}
		else {
			this.powerLabel.enabled       = false;
			this.powerLabelShadow.enabled = false;
		}
	}
}
