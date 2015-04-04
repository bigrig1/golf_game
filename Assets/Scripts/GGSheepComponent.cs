//
// The component that manages a sheep.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGSheepComponent: MonoBehaviour {
	/* Responding to collisions. */
	
	public void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.GetComponent<GGBallComponent>() != null) {
			// TODO: Do something interesting.
			GameObject.Destroy(this.gameObject);
		}
	}
}
