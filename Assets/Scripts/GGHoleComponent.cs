//
// The component that manages a platform's hole.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGHoleComponent: MonoBehaviour {
	/* Checking for ball containment. */
	
	public bool containsBall { get; private set; }
	
	/* Responding to collisions. */
	
	public void OnTriggerEnter2D(Collider2D collider) {
		this.containsBall = true;
	}
	
	public void OnTriggerStay2D(Collider2D collider) {
		this.containsBall = true;
	}
	
	/* Updating. */
	
	public void FixedUpdate() {
		this.containsBall = false;
	}
}
