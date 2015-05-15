//
// A static class that manages some save data operations.
//

using System.Collections;
using UnityEngine;

public abstract class GGSaveData {
	public static float GetBallX() {
		return PlayerPrefs.GetFloat(GGSaveData.GetScopedKey("Ball X"));
	}
	
	public static void SetBallX(float x) {
		PlayerPrefs.SetFloat(GGSaveData.GetScopedKey("Ball X"), x);
	}
	
	public static float GetBallY() {
		return PlayerPrefs.GetFloat(GGSaveData.GetScopedKey("Ball Y"));
	}
	
	public static void SetBallY(float y) {
		PlayerPrefs.SetFloat(GGSaveData.GetScopedKey("Ball Y"), y);
	}
	
	public static bool HasBallPosition() {
		return PlayerPrefs.HasKey(GGSaveData.GetScopedKey("Ball X")) && PlayerPrefs.HasKey(GGSaveData.GetScopedKey("Ball Y"));
	}
	
	public static int GetSheepCount() {
		return PlayerPrefs.GetInt(GGSaveData.GetScopedKey("Sheep Count"), 0);
	}
	
	public static void SetSheepCount(int sheepCount) {
		PlayerPrefs.SetInt(GGSaveData.GetScopedKey("Sheep Count"), sheepCount);
	}
	
	public static int GetCurrentMapIndex() {
		return PlayerPrefs.GetInt(GGSaveData.GetScopedKey("Current Map Index"), 0);
	}
	
	public static void SetCurrentMapIndex(int currentMapIndex) {
		PlayerPrefs.SetInt(GGSaveData.GetScopedKey("Current Map Index"), currentMapIndex);
	}
	
	public static bool GetSheepHitFlag(string id) {
		return PlayerPrefs.HasKey(GGSaveData.GetScopedKey("Sheep " + id));
	}
	
	public static void SetSheepHitFlag(string id) {
		PlayerPrefs.SetInt(GGSaveData.GetScopedKey("Sheep " + id), 1);
	}
	
	public static void DeleteSheepHitFlag(string id) {
		PlayerPrefs.DeleteKey(GGSaveData.GetScopedKey("Sheep " + id));
	}
	
	private static string GetScopedKey(string key) {
		return GGSaveData.GetScopeForMode(GGGameSceneComponent.mode) + " " + key;
	}
	
	private static string GetScopeForMode(GGGameMode mode) {
		switch (mode) {
			case GGGameMode.Zen:     return "Zen";
			case GGGameMode.Regular: return "Regular";
			case GGGameMode.Hard:    return "Hard";
		}
		
		return "";
	}
}
