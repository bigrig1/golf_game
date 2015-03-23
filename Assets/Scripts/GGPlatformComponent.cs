//
// The component that manages platform objects.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGPlatformComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		var physicsComponent = GGGameSceneComponent.instance.physicsComponent;
		
		foreach (var collider in this.colliders) {
			var name = collider.gameObject.name;
			
			switch (name) {
				case "Grass": collider.sharedMaterial = physicsComponent.grassMaterial; break;
				case "Dirt":  collider.sharedMaterial = physicsComponent.dirtMaterial;  break;
				case "Sand":  collider.sharedMaterial = physicsComponent.sandMaterial;  break;
				case "Rock":  collider.sharedMaterial = physicsComponent.rockMaterial;  break;
				default:      Debug.LogError("Unhandled platform child name: " + name); break;
			}
		}
	}
	
	/* Getting child objects and components. */
	
	// The colliders of each portion of the platform.
	public List<PolygonCollider2D> colliders { get {
		if (_colliders == null) {
			this.LoadCollidersAndHolePoints();
		}
		
		return _colliders;
	} }
	
	private List<PolygonCollider2D> _colliders;
	
	public List<Vector2> holePoints { get {
		if (_holePoints == null) {
			this.LoadCollidersAndHolePoints();
		}
		
		return _holePoints;
	} }
	
	private List<Vector2> _holePoints;
	
	private void LoadCollidersAndHolePoints() {
		_colliders     = new List<PolygonCollider2D>();
		_holePoints    = new List<Vector2>();
		var transform  = this.transform;
		var childCount = transform.childCount;
		
		for (var i = 0; i < childCount; i += 1) {
			var childTransform = transform.GetChild(i);
			
			if (childTransform.gameObject.name == "Hole") {
				_holePoints.Add(childTransform.localPosition);
				Debug.Log(childTransform.localPosition);
			}
			else {
				_colliders.Add(childTransform.GetComponent<PolygonCollider2D>());
			}
		}
	}
	
	/* Getting information about the platform. */
	
	// The size of the rectangle that encloses the platform.
	public Vector2 size { get {
		if (_size == Vector2.zero) {
			var minX = float.MaxValue;
			var maxX = float.MinValue;
			var minY = float.MaxValue;
			var maxY = float.MinValue;
			
			foreach (var collider in this.colliders) {
				foreach (var point in collider.points) {
					minX = Mathf.Min(minX, point.x);
					maxX = Mathf.Max(maxX, point.x);
					minY = Mathf.Min(minY, point.y);
					maxY = Mathf.Max(maxY, point.y);
				}
			}
			
			_size = new Vector2(maxX - minX, maxY - minY);
		}
		
		return _size;
	} }
	
	private Vector2 _size;
	
	// The platform's size class, which is based on its width.
	public GGPlatformSizeClass sizeClass { get {
		var width = this.size.x;
		
		if (width < 4.0f) {
			return GGPlatformSizeClass.Small;
		}
		else if (width < 7.0f) {
			return GGPlatformSizeClass.Medium;
		}
		else {
			return GGPlatformSizeClass.Large;
		}
	} }
}
