//
// The component that manages the main menu's UI.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGMainMenuUIComponent: MonoBehaviour {
	/* Responding to button presses. */
	
	public void ZenModeButtonWasPressed() {
		GGGameSceneComponent.mode = GGGameMode.Zen;
		Application.LoadLevel("Game");
	}
	
	public void RegularModeButtonWasPressed() {
		GGGameSceneComponent.mode = GGGameMode.Regular;
		Application.LoadLevel("Game");
	}
	
	public void HardModeButtonWasPressed() {
		GGGameSceneComponent.mode = GGGameMode.Hard;
		Application.LoadLevel("Game");
	}
	
	/* Updating. */
	
	public void FixedUpdate() {
		// Debug.Log("ZEN: " + GGSaveData.HasSaveData(GGGameMode.Zen));
		// Debug.Log("REGULAR: " + GGSaveData.HasSaveData(GGGameMode.Regular));
		// Debug.Log("HARD: " + GGSaveData.HasSaveData(GGGameMode.Hard));
	}
}
