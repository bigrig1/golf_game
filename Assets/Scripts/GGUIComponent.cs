//
// The component that manages the game's UI.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GGUIComponent: MonoBehaviour {
	/* Accessing UI components. */
	
	public Text sheepCountLabel;
	public Text sheepCountLabelShadow;
	
	public Text holeLabel;
	public Text holeLabelShadow;
	
	public Image strokeCountIcon;
	public Text strokeCountLabel;
	public Text strokeCountLabelShadow;
	
	public Button undoButton;
	
	/* Initializing. */
	
	public void Start() {
		var strokeCountIsEnabled = GGGameSceneComponent.mode != GGGameMode.Zen;
		strokeCountIcon.gameObject.SetActive(strokeCountIsEnabled);
		strokeCountLabel.gameObject.SetActive(strokeCountIsEnabled);
		strokeCountLabelShadow.gameObject.SetActive(strokeCountIsEnabled);
	}
	
	/* Updating. */
	
	public void FixedUpdate() {
		var gameSceneComponent     = GGGameSceneComponent.instance;
		var inputComponent         = gameSceneComponent.inputComponent;
		var sheepCountString       = "" + gameSceneComponent.sheepCount;
		var holeString             = "Hole " + (gameSceneComponent.mapComponent.currentMapIndex + 1);
		sheepCountLabel.text       = sheepCountString;
		sheepCountLabelShadow.text = sheepCountString;
		holeLabel.text             = holeString;
		holeLabelShadow.text       = holeString;
		undoButton.interactable    = !inputComponent.ballHasBeenHit && gameSceneComponent.ballComponent.canRestoreUndoPosition;
		
		if (GGGameSceneComponent.mode != GGGameMode.Zen) {
			var strokeCountString       = "" + gameSceneComponent.remainingStrokeCount;
			strokeCountLabel.text       = strokeCountString;
			strokeCountLabelShadow.text = strokeCountString;
		}
	}
}
