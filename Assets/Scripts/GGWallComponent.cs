//
// The component that manages a section of wall.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGWallComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.collider2D.sharedMaterial = GGGameSceneComponent.instance.physicsComponent.rockMaterial;
		this.SetSheepSpawnPointsIfNeeded();
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
	
	// The points in the local space of this wall that sheep can spawn, if any.
	public List<Vector2> sheepSpawnPoints { get {
		this.SetSheepSpawnPointsIfNeeded();
		return _sheepSpawnPoints;
	} }
	
	private List<Vector2> _sheepSpawnPoints;
	
	private void SetSheepSpawnPointsIfNeeded() {
		if (_sheepSpawnPoints == null) {
			_sheepSpawnPoints = new List<Vector2>();
			var transform     = this.transform;
			var childCount    = transform.childCount;
			
			for (var i = 0; i < childCount; i += 1) {
				var childTransform = transform.GetChild(i);
				
				if (childTransform.gameObject.name == "Sheep Spawn") {
					this.sheepSpawnPoints.Add(childTransform.localPosition);
					GameObject.Destroy(childTransform.gameObject);
				}
			}
		}
	}
	
	/* Spawning sheep. */
	
	// Spawns a sheep if possible. Returns the sheep component if it was created.
	public GGSheepComponent SpawnSheep(System.Random random) {
		if (this.sheepSpawnPoints.Count == 0) {
			return null;
		}
		
		var isOnRightSide = this.transform.localScale.x < 0.0f;
		var sheepOffset   = new Vector2(-0.625f, 0.825f);
		
		if (isOnRightSide) {
			sheepOffset.x *= -1.0f;
		}
		
		var sheep           = GameObject.Instantiate(Resources.Load("Prefabs/Sheep")) as GameObject;
		var sheepTransform  = sheep.transform;
		var sheepComponent  = sheep.GetComponent<GGSheepComponent>();
		var spawnPointIndex = random.Next(0, this.sheepSpawnPoints.Count);
		sheep.name          = "Sheep";
		sheepTransform.SetParent(this.transform, false);
		sheepTransform.localPosition = sheepSpawnPoints[spawnPointIndex] + sheepOffset;
		
		if (isOnRightSide) {
			var scale                 = sheepTransform.localScale;
			scale.x                  *= -1.0f;
			sheepTransform.localScale = scale;
		}
		
		return sheepComponent;
	}
}
