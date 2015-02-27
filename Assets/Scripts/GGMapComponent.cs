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
		var mapWidth             = GGMapComponent.mapWidth;
		var mapHeight            = GGMapComponent.mapHeight;
		var horizontalMapPadding = GGMapComponent.horizontalMapPadding;
		var usableMapWidth       = mapWidth - horizontalMapPadding * 2.0f;
		var wallY                = 0.0f;
		
		while (wallY < mapHeight) {
			wallY += this.AddWall(wallY + yOffset, true);
		}
		
		wallY = 0.0f;
		
		while (wallY < mapHeight) {
			wallY += this.AddWall(wallY + yOffset, false);
		}
		
		// TODO: This should be random between ~4-6, probably skewed so that the higher the map
		// index, the more likely the number will be lower.
		var sectionCount      = 5;
		var divisionCount     = sectionCount - 1;
		var divisionPoints    = new float[divisionCount];
		var baseSectionHeight = mapHeight / (float)sectionCount;
		
		for (var i = 0; i < divisionCount; i += 1) {
			// TODO: Plus or minus some random offset for variety.
			divisionPoints[i] = (float)(i + 1) * baseSectionHeight;
		}
		
		var sectionY = yOffset;
		
		for (var i = 0; i < sectionCount; i += 1) {
			var sectionHeight      = i == sectionCount - 1 ? mapHeight - sectionY : divisionPoints[i] - sectionY;
			var innerSectionHeight = sectionHeight * GGMapComponent.innerSectionHeightRatio;
			var sectionYOffset     = (sectionHeight - innerSectionHeight) / 2.0f;
			var innerBounds        = new Rect(-usableMapWidth / 2.0f, sectionY + sectionYOffset, usableMapWidth, innerSectionHeight);
			
			// TEMP: Need to figure out how to grab platform pieces.
			// TODO: Pick a random spot inside the usable bounds, but make sure that platforms don't
			// overlap.
			var platform                     = GameObject.Instantiate(this.platformPrototypes[0]) as GameObject;
			var platformComponent            = platform.GetComponent<GGPlatformComponent>();
			var platformSize                 = platformComponent.size;
			var usableBounds                 = innerBounds;
			usableBounds.x                  += platformSize.x / 2.0f;
			usableBounds.y                  += platformSize.y / 2.0f;
			usableBounds.width              -= platformSize.x;
			usableBounds.height             -= platformSize.y;
			platform.name                    = "Platform";
			platform.transform.localPosition = new Vector3(usableBounds.x, usableBounds.yMax, 0.0f);
			
			// TODO: Random chance to flip the platform horizontally.
			
			platform.SetActive(true);
			this.platformComponents.Add(platformComponent);
			
			if (i < divisionCount) {
				sectionY = divisionPoints[i];
			}
		}
		
		// The ground should only be active on the very first level. If we're generating any level
		// other than the first one, then we've made it past the first level.
		this.groundComponent.gameObject.SetActive(mapIndex == 0);
	}
	
	// Adds a wall segment at the given Y position and returns the height of the segment that was
	// added.
	private float AddWall(float y, bool isOnLeftSide) {
		var x = GGMapComponent.mapWidth / 2.0f;
		
		if (isOnLeftSide) {
			x = -x;
		}
		
		// TEMP: Need to figure out how to grab wall pieces.
		var wall                     = GameObject.Instantiate(this.wallPrototypes[0]) as GameObject;
		var wallComponent            = wall.GetComponent<GGWallComponent>();
		var wallHeight               = wallComponent.height;
		wall.name                    = "Wall";
		wall.transform.localPosition = new Vector3(x, y + wallHeight / 2.0f, 0.0f);
		
		if (!isOnLeftSide) {
			wall.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
		}
		
		wall.SetActive(true);
		this.wallComponents.Add(wallComponent);
		return wallHeight;
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
	
	// The width of each map. This defines where walls and platforms are placed.
	public const float mapWidth = 28.0f;
	
	// The full height of each map, from the bottom of the screen to the top.
	public const float mapHeight = 48.0f;
	
	// The amount of padding on the sides of the map, which is used for positioning platforms so
	// that they don't overlap the walls.
	public const float horizontalMapPadding = 3.5f;
	
	// The ratio of the height of the inner bounds of a platform section to the full height of the
	// section. If this value is 1.0, the inner section will be just as tall as the outer section,
	// which means platforms from each section will be able to touch each other. Reducing the ratio
	// ensures that there will be more space between the platforms of each section, but reduces the
	// variety in vertical positioning of platforms within a section.
	public const float innerSectionHeightRatio = 0.65f;
	
	// The minimum and maximum number of platform sections that can be used for an individual level.
	public const int minSectionCount = 3;
	public const int maxSectionCount = 6;
}
