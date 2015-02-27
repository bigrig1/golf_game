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
			_colliders     = new List<PolygonCollider2D>();
			var transform  = this.transform;
			var childCount = transform.childCount;
			
			for (var i = 0; i < childCount; i += 1) {
				_colliders.Add(transform.GetChild(i).GetComponent<PolygonCollider2D>());
			}
		}
		
		return _colliders;
	} }
	
	private List<PolygonCollider2D> _colliders;
	
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
}
