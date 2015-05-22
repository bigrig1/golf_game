//
// The component that manages an individual cloud.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGCloudComponent: MonoBehaviour {
	/* Configuring the cloud. */
	
	public float velocity;
	
	/* Updating. */
	
	public void FixedUpdate() {
		var position            = this.transform.position;
		position.x             += velocity * Time.deltaTime;
		this.transform.position = position;
	}
}
