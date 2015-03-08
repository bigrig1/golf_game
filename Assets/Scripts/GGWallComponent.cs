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
	
	// The wall's size class, which is based on its height.
	public GGWallSizeClass sizeClass { get {
		var height = Mathf.Round(this.height);
		
		if (height == GGWallSizeClass.Small.GetHeight()) {
			return GGWallSizeClass.Small;
		}
		else if (height == GGWallSizeClass.Medium.GetHeight()) {
			return GGWallSizeClass.Medium;
		}
		else if (height == GGWallSizeClass.Large.GetHeight()) {
			return GGWallSizeClass.Large;
		}
		
		Debug.LogError("Tried to get size class of a wall whose height did not correspond to any size class.");
		
		return GGWallSizeClass.Small;
	} }
}
