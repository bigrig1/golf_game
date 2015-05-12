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
		
		this.LoadCollidersIfNeeded();
	}
	
	/* Getting child objects and components. */
	
	// The colliders of each portion of the platform.
	public List<PolygonCollider2D> colliders { get {
		this.LoadCollidersIfNeeded();
		return _colliders;
	} }
	
	private List<PolygonCollider2D> _colliders;
	
	// The platform component's hole object if it has one.
	public GameObject hole { get {
		this.LoadCollidersIfNeeded();
		return _hole;
	} }
	
	private GameObject _hole; 
	
	private void LoadCollidersIfNeeded() {
		if (_colliders == null) {
			_colliders     = new List<PolygonCollider2D>();
			var transform  = this.transform;
			var childCount = transform.childCount;
			
			for (var i = 0; i < childCount; i += 1) {
				var childTransform = transform.GetChild(i);
				
				if (childTransform.gameObject.name == "Hole") {
					_hole = childTransform.gameObject;
				}
				else {
					_colliders.Add(childTransform.GetComponent<PolygonCollider2D>());
				}
			}
		}
	}
	
	/* Getting information about the platform. */
	
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
	
	// The size of the rectangle that encloses the platform.
	public Vector2 size { get {
		if (_size == Vector2.zero) {
			this.CalculateSize();
		}
		
		return _size;
	} }
	
	private Vector2 _size;
	
	// The lowest X-point of the platform in its own local space.
	public float lowestLocalX { get {
		if (_lowestLocalX == float.MaxValue) {
			this.CalculateSize();
		}
		
		return _lowestLocalX;
	} }
	
	private float _lowestLocalX = float.MaxValue;
	
	// The lowest Y-point of the platform in its own local space.
	public float highestLocalX { get {
		if (_highestLocalX == float.MinValue) {
			this.CalculateSize();
		}
		
		return _highestLocalX;
	} }
	
	private float _highestLocalX = float.MinValue;
	
	// The lowest Y-point of the platform in its own local space.
	public float lowestLocalY { get {
		if (_lowestLocalY == float.MaxValue) {
			this.CalculateSize();
		}
		
		return _lowestLocalY;
	} }
	
	private float _lowestLocalY = float.MaxValue;
	
	// The highest Y-point of the platform in its own local space.
	public float highestLocalY { get {
		if (_highestLocalY == float.MinValue) {
			this.CalculateSize();
		}
		
		return _highestLocalY;
	} }
	
	private float _highestLocalY = float.MinValue;
	
	// The lowest X-point of the platform in world space.
	public float lowestX { get {
		return this.transform.position.x + this.lowestLocalX;
	} }
	
	// The highest X-point of the platform in world space.
	public float highestX { get {
		return this.transform.position.x + this.highestLocalX;
	} }
	
	// The lowest Y-point of the platform in world space.
	public float lowestY { get {
		return this.transform.position.y + this.lowestLocalY;
	} }
	
	// The highest Y-point of the platform in world space.
	public float highestY { get {
		return this.transform.position.y + this.highestLocalY;
	} }
	
	private void CalculateSize() {
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
		
		_size          = new Vector2(maxX - minX, maxY - minY);
		_lowestLocalX  = minX;
		_highestLocalX = maxX;
		_lowestLocalY  = minY;
		_highestLocalY = maxY;
	}
	
	/* Spawning sheep. */
	
	// Spawns a sheep hanging underneath the platform.
	public void SpawnSheep(System.Random random) {
		var sheepOffset          = new Vector2(-0.625f, -1.5f);
		var sheep                = GameObject.Instantiate(Resources.Load("Prefabs/Sheep")) as GameObject;
		var sheepComponent       = sheep.GetComponent<GGSheepComponent>();
		var sheepTransform       = sheep.transform;
		var normalizedX          = 0.2f + 0.6f * (float)random.NextDouble();
		var localX               = this.lowestLocalX + this.size.x * normalizedX;
		sheep.name               = "Sheep";
		sheepComponent.isHanging = true;
		sheepTransform.SetParent(this.transform, false);
		
		var raycastHit = Physics2D.Raycast(
			new Vector2(this.transform.position.x + localX, this.lowestY - 0.05f),
			new Vector2(0.0f, 1.0f)
		);
		
		if (raycastHit) {
			sheepTransform.localPosition = new Vector2(localX, raycastHit.point.y - this.transform.position.y) + sheepOffset;
		}
	}
}
