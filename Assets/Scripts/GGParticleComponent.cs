//
// A component that's on a particle.
//

using System.Collections;
using UnityEngine;

public class GGParticleComponent: MonoBehaviour {
	private float duration;
	
	public void Start() {
		this.duration = Random.Range(1.5f, 3.0f);
	}
	
	public void FixedUpdate() {
		this.duration -= Time.deltaTime;
		
		if (this.duration <= 0.0f) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
