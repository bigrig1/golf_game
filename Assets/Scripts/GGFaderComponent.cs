//
// A component that performs screen fades.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GGFaderComponent: MonoBehaviour {
	/* Fading. */
	
	// Set this to true before switching scenes to fade in.
	public static bool shouldFadeIn = false;
	
	// Call this to fade to a scene.
	public static void FadeOut(string scene) {
		var fader           = GameObject.FindObjectOfType<GGFaderComponent>();
		fader.isFadingOut   = true;
		fader.sceneToFadeTo = scene;
	}
	
	/* Initializing. */
	
	public void Start() {
		this.image                    = this.GetComponent<Image>();
		this.image.color              = new Color(0.0f, 0.0f, 0.0f, GGFaderComponent.shouldFadeIn ? 1.0f : 0.0f);
		GGFaderComponent.shouldFadeIn = false;
	}
	
	/* Getting components. */
	
	private Image image;
	
	/* Updating. */
	
	public bool isFadingOut = false;
	public string sceneToFadeTo;
	
	public void Update() {
		var color = this.image.color;
		
		if (this.isFadingOut) {
			if (color.a < 1.0f) {
				color.a         += 1.8f * Time.deltaTime;
				this.image.color = color;
			}
			else {
				Application.LoadLevel(this.sceneToFadeTo);
			}
		}
		else if (color.a > 0.0f) {
			color.a         -= 2.4f * Time.deltaTime;
			this.image.color = color;
		}
	}
}
