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
	
	/* Updating. */
	
	public void FixedUpdate() {
		var gameSceneComponent     = GGGameSceneComponent.instance;
		var sheepCountString       = "" + gameSceneComponent.sheepCount;
		sheepCountLabel.text       = sheepCountString;
		sheepCountLabelShadow.text = sheepCountString;
	}
}
