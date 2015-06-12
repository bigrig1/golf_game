//
// The component that manages a platform's hole.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGHoleComponent: MonoBehaviour {
	// MARK: Responding to events
	
	// Plugs the hole by adding collision to the top of it, preventing the ball from entering it.
	public void Plug() {
		this.transform.Find("Plug").gameObject.SetActive(true);
		this.spriteRenderer = this.GetComponent<SpriteRenderer>();
	}
	
	private SpriteRenderer spriteRenderer;
	
	public void Update() {
		if (this.spriteRenderer != null) {
			var color = this.spriteRenderer.material.color;
			
			if (color.a > 0.0f) {
				color.a                           -= Time.deltaTime * 4.75f;
				this.spriteRenderer.material.color = color;
			}
		}
	}
}
