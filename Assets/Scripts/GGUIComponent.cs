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
	
	public Text strokeCountLabel;
	public Text strokeCountLabelShadow;
	
	/* Updating. */
	
	public void FixedUpdate() {
		var gameSceneComponent     = GGGameSceneComponent.instance;
		var sheepCountString       = "" + gameSceneComponent.sheepCount;
		var holeString             = "Hole " + (gameSceneComponent.mapComponent.currentMapIndex + 1);
		var strokeCountString      = "0"; // TODO
		sheepCountLabel.text       = sheepCountString;
		sheepCountLabelShadow.text = sheepCountString;
		holeLabel.text             = holeString;
		holeLabelShadow.text       = holeString;
		strokeCountLabel.text      = strokeCountString;
		strokeCountLabel.text      = strokeCountString;
	}
}
