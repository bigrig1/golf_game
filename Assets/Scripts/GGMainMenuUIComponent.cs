//
// The component that manages the main menu's UI.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GGMainMenuUIComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.VerifySaveData();
		
		this.zenResetButton.SetActive(GGSaveData.HasSaveData(GGGameMode.Zen));
		this.regularResetButton.SetActive(GGSaveData.HasSaveData(GGGameMode.Regular));
		this.hardResetButton.SetActive(GGSaveData.HasSaveData(GGGameMode.Hard));
		
		if (GGSaveData.HasHighScore(GGGameMode.Zen)) {
			this.zenHoleScoreLabel.GetComponent<Text>().text  = "" + GGSaveData.GetHoleHighScore(GGGameMode.Zen);
			this.zenSheepScoreLabel.GetComponent<Text>().text = "" + GGSaveData.GetSheepHighScore(GGGameMode.Zen);
		}
		else {
			this.zenHoleScoreIcon.SetActive(false);
			this.zenSheepScoreIcon.SetActive(false);
			this.zenHoleScoreLabel.SetActive(false);
			this.zenSheepScoreLabel.SetActive(false);
		}
		
		if (GGSaveData.HasHighScore(GGGameMode.Regular)) {
			this.regularHoleScoreLabel.GetComponent<Text>().text  = "" + GGSaveData.GetHoleHighScore(GGGameMode.Regular);
			this.regularSheepScoreLabel.GetComponent<Text>().text = "" + GGSaveData.GetSheepHighScore(GGGameMode.Regular);
		}
		else {
			this.regularHoleScoreIcon.SetActive(false);
			this.regularSheepScoreIcon.SetActive(false);
			this.regularHoleScoreLabel.SetActive(false);
			this.regularSheepScoreLabel.SetActive(false);
		}
		
		if (GGSaveData.HasHighScore(GGGameMode.Hard)) {
			this.hardHoleScoreLabel.GetComponent<Text>().text  = "" + GGSaveData.GetHoleHighScore(GGGameMode.Hard);
			this.hardSheepScoreLabel.GetComponent<Text>().text = "" + GGSaveData.GetSheepHighScore(GGGameMode.Hard);
		}
		else {
			this.hardHoleScoreIcon.SetActive(false);
			this.hardSheepScoreIcon.SetActive(false);
			this.hardHoleScoreLabel.SetActive(false);
			this.hardSheepScoreLabel.SetActive(false);
		}
	}
	
	private void VerifySaveData() {
		if (GGSaveData.HasSaveData(GGGameMode.Regular) && GGSaveData.GetRemainingStrokeCountMode(GGGameMode.Regular) <= 0) {
			GGSaveData.DeleteSaveData(GGGameMode.Regular);
		}
		
		if (GGSaveData.HasSaveData(GGGameMode.Hard) && GGSaveData.GetRemainingStrokeCountMode(GGGameMode.Hard) <= 0) {
			GGSaveData.DeleteSaveData(GGGameMode.Hard);
		}
	}
	
	/* Accessing UI. */
	
	public GameObject zenResetButton;
	public GameObject regularResetButton;
	public GameObject hardResetButton;
	
	public GameObject zenHoleScoreIcon;
	public GameObject zenSheepScoreIcon;
	public GameObject zenHoleScoreLabel;
	public GameObject zenSheepScoreLabel;
	
	public GameObject regularHoleScoreIcon;
	public GameObject regularSheepScoreIcon;
	public GameObject regularHoleScoreLabel;
	public GameObject regularSheepScoreLabel;
	
	public GameObject hardHoleScoreIcon;
	public GameObject hardSheepScoreIcon;
	public GameObject hardHoleScoreLabel;
	public GameObject hardSheepScoreLabel;
	
	/* Responding to button presses. */
	
	public void ZenModeButtonWasPressed() {
		GGGameSceneComponent.mode = GGGameMode.Zen;
		GGFaderComponent.shouldFadeIn = true;
		GGFaderComponent.FadeOut("Game");
	}
	
	public void RegularModeButtonWasPressed() {
		GGGameSceneComponent.mode = GGGameMode.Regular;
		GGFaderComponent.shouldFadeIn = true;
		GGFaderComponent.FadeOut("Game");
	}
	
	public void HardModeButtonWasPressed() {
		GGGameSceneComponent.mode = GGGameMode.Hard;
		GGFaderComponent.shouldFadeIn = true;
		GGFaderComponent.FadeOut("Game");
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
