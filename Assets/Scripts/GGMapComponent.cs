//
// The component that manages the map including procedural generation of each screen.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGMapComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Awake() {
		
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
	public List<GGPlatformComponent> platformComponentPrototypes = new List<GGPlatformComponent>();
	
	// The wall components for the wall prototypes.
	[HideInInspector]
	public List<GGWallComponent> wallComponentPrototypes = new List<GGWallComponent>();
	
	/* Building maps. */
	
	// Procedurally generates a map by creating all the platforms and walls for it with the given Y
	// offset. You'll always get the same map for a given map index.
	public void BuildMap(int mapIndex, float yOffset) {
		// TODO
	}
	
	/* Loading game objects. */
	
	public void LoadPlatformPrototype(GameObject platform) {
		this.platformComponentPrototypes.Add(platform.GetComponent<GGPlatformComponent>());
	}
	
	public void LoadWallPrototype(GameObject wall) {
		this.wallComponentPrototypes.Add(wall.GetComponent<GGWallComponent>());
	}
	
	public void LoadGround(GameObject ground) {
		this.groundComponent = ground.GetComponent<GGGroundComponent>();
		this.groundCollider  = ground.collider2D;
	}
	
	/* Getting configuration values. */
	
	// TODO
}
