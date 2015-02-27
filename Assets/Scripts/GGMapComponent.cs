//
// The component that manages the map including procedural generation of each screen.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGMapComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Awake() {
		// Nothing yet.
	}
	
	/* Accessing game objects and components. */
	
	// The ground component.
	public GGGroundComponent groundComponent { get; private set; }
	
	// The ground object's collider.
	public Collider2D groundCollider { get; private set; }
	
	// The list of platform components that are currently being used in the map.
	[HideInInspector]
	public List<GGPlatformComponent> platformComponents = new List<GGPlatformComponent>();
	
	// The list of platform components that were used in the previous map. We need to keep them here
	// during transitions, but they'll be destroyed immediately after the transition completes.
	[HideInInspector]
	public List<GGPlatformComponent> oldPlatformComponents = new List<GGPlatformComponent>();
	
	// The list of wall components that are currently being used in the map.
	[HideInInspector]
	public List<GGWallComponent> wallComponents = new List<GGWallComponent>();
	
	// The list of wall components that were used in the previous map. We need to keep them here
	// during transitions, but they'll be destroyed immediately after the transition completes.
	[HideInInspector]
	public List<GGWallComponent> oldWallComponents = new List<GGWallComponent>();
	
	/* Managing map component prototypes. */
	
	// The platform components for the platform prototypes.
	[HideInInspector]
	public List<GameObject> platformPrototypes = new List<GameObject>();
	
	// The wall components for the wall prototypes.
	[HideInInspector]
	public List<GameObject> wallPrototypes = new List<GameObject>();
	
	/* Building maps. */
	
	// Procedurally generates a map by creating all the platforms and walls for it with the given Y
	// offset. You'll always get the same map for a given map index.
	public void BuildMap(int mapIndex, float yOffset) {
		var mapWidth   = GGMapComponent.mapWidth;
		var leftWallX  = -mapWidth / 2.0f;
		// var rightWallX =  mapWidth / 2.0f;
		
		// TEMP: Need to figure out how to grab wall pieces.
		// while (this.leftWallY < maxY) {
		// 	this.leftWallY += this.AddWall(this.leftWallY, true);
		// }
		
		var wallY               = yOffset;
		var wall                = GameObject.Instantiate(this.wallPrototypes[0]) as GameObject;
		var wallComponent       = wall.GetComponent<GGWallComponent>();
		var wallHeight          = wallComponent.height;
		wall.name               = "Wall";
		wall.transform.position = new Vector3(leftWallX, wallY + wallHeight / 2.0f, 0.0f);
		wall.SetActive(true);
		this.wallComponents.Add(wallComponent);
		
		// The ground should only be active on the very first level. If we're generating any level
		// other than the first one, then we've made it past the first level.
		this.groundComponent.gameObject.SetActive(mapIndex == 0);
	}
	
	private float AddWall(float y, bool isOnLeftSide) {
		// TODO
		return 0.0f;
	}
	
	/* Loading game objects. */
	
	public void LoadPlatformPrototype(GameObject platform) {
		this.platformPrototypes.Add(platform);
		platform.SetActive(false);
	}
	
	public void LoadWallPrototype(GameObject wall) {
		this.wallPrototypes.Add(wall);
		wall.SetActive(false);
	}
	
	public void LoadGround(GameObject ground) {
		this.groundComponent = ground.GetComponent<GGGroundComponent>();
		this.groundCollider  = ground.collider2D;
	}
	
	/* Getting configuration values. */
	
	public const float mapWidth     = 28.0f;
	public const float mapHeight    = 60.0f;
	public const float groundHeight = 3.0f;
}
