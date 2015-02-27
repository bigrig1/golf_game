//
// The component that manages a section of wall.
//

using System.Collections;
using UnityEngine;

public class GGWallComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.collider2D.sharedMaterial = GGGameSceneComponent.instance.physicsComponent.rockMaterial;
	}
	
	/* Getting information about the wall. */
	
	// The height of the wall in game units.
	public float height { get {
		if (_height == 0.0f) {
			_height = this.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		}
		
		return _height;
	} }
	
	private float _height;
}
