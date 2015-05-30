//
// The component that manages the main menu's UI.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGMainMenuUIComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.zenResetButton.SetActive(GGSaveData.HasSaveData(GGGameMode.Zen));
		this.regularResetButton.SetActive(GGSaveData.HasSaveData(GGGameMode.Regular));
		this.hardResetButton.SetActive(GGSaveData.HasSaveData(GGGameMode.Hard));
		this.GetComponent<Canvas>().scaleFactor = GGBridge.GetScreenPixelScale();
	}
	
	/* Accessing UI. */
	
	public GameObject zenResetButton;
	public GameObject regularResetButton;
	public GameObject hardResetButton;
	
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
	
	public void ZenResetButtonWasPressed() {
		GGSaveData.DeleteSaveData(GGGameMode.Zen);
		this.zenResetButton.SetActive(false);
	}
	
	public void RegularResetButtonWasPressed() {
		GGSaveData.DeleteSaveData(GGGameMode.Regular);
		this.regularResetButton.SetActive(false);
	}
	
	public void HardResetButtonWasPressed() {
		GGSaveData.DeleteSaveData(GGGameMode.Hard);
		this.hardResetButton.SetActive(false);
	}
}
