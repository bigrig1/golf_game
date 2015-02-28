//
// The component that manages the map including procedural generation of each screen.
//

using System;
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
	
	// The platform components for the platform prototypes, grouped by size class.
	[HideInInspector]
	public List<GameObject> smallPlatformPrototypes  = new List<GameObject>();
	public List<GameObject> mediumPlatformPrototypes = new List<GameObject>();
	public List<GameObject> largePlatformPrototypes  = new List<GameObject>();
	
	// The wall components for the wall prototypes.
	[HideInInspector]
	public List<GameObject> wallPrototypes = new List<GameObject>();
	
	/* Building maps. */
	
	// Procedurally generates a map by creating all the platforms and walls for it with the given Y
	// offset. You'll always get the same map for a given map index.
	public void BuildMap(int mapIndex, float yOffset) {
		var random = new System.Random(168403912 + mapIndex);
		this.AddWalls(random, yOffset);
		this.AddPlatforms(mapIndex, random, yOffset);
		
		// The ground should only be active on the very first map. If we're generating any map other
		// than the first one, then we've made it past the first map.
		this.groundComponent.gameObject.SetActive(mapIndex == 0);
	}
	
	// Adds all of the walls for the map.
	private void AddWalls(System.Random random, float yOffset) {
		var mapHeight = GGMapComponent.mapHeight;
		var wallY     = 0.0f;
		
		while (wallY < mapHeight) {
			wallY += this.AddWall(wallY + yOffset, true, random);
		}
		
		wallY = 0.0f;
		
		while (wallY < mapHeight) {
			wallY += this.AddWall(wallY + yOffset, false, random);
		}
	}
	
	// Adds a wall segment at the given Y position and returns the height of the segment that was
	// added.
	private float AddWall(float y, bool isOnLeftSide, System.Random random) {
		var x = GGMapComponent.mapWidth / 2.0f;
		
		if (isOnLeftSide) {
			x = -x;
		}
		
		var wallIndex                = random.Next(this.wallPrototypes.Count);
		var wall                     = GameObject.Instantiate(this.wallPrototypes[wallIndex]) as GameObject;
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
	
	// Adds all of the platforms for the map.
	private void AddPlatforms(int mapIndex, System.Random random, float yOffset) {
		var usableMapHeight             = GGMapComponent.mapHeight - GGMapComponent.topMapPadding;
		var platformArrangements        = this.PlatformArrangementsForMapIndex(mapIndex);
		var sectionCount                = random.Next(GGMapComponent.minSectionCount, GGMapComponent.maxSectionCount + 1);
		var sectionCountReductionChance = (float)mapIndex / 500.0f;
		
		if (sectionCount > GGMapComponent.minSectionCount && (float)random.NextDouble() < sectionCountReductionChance) {
			sectionCount -= 1;
		}
		
		var sectionMaxYs            = new float[sectionCount];
		var baseSectionHeight       = usableMapHeight / (float)sectionCount;
		var divisionAdjustmentRange = baseSectionHeight / 5.0f;
		var totalPlatformFrequency  = 0.0f;
		
		foreach (var platformArrangement in platformArrangements) {
			totalPlatformFrequency += platformArrangement.frequency;
		}
		
		for (var i = 0; i < sectionCount - 1; i += 1) {
			sectionMaxYs[i] = (float)(i + 1) * baseSectionHeight + divisionAdjustmentRange + (float)(random.NextDouble() - 0.5);
		}
		
		sectionMaxYs[sectionCount - 1] = usableMapHeight;
		
		for (var i = 0; i < sectionCount; i += 1) {
			this.AddPlatformSection(i, yOffset, sectionMaxYs, sectionCount, platformArrangements, totalPlatformFrequency, random);
		}
	}
	
	// Adds all the platforms for the given section.
	private void AddPlatformSection(int index, float yOffset, float[] maxYs, int sectionCount, GGPlatformArrangement[] arrangements, float totalFrequency, System.Random random) {
		var usableMapWidth                        = GGMapComponent.mapWidth - GGMapComponent.horizontalMapPadding * 2.0f;
		var y                                     = index == 0 ? yOffset : maxYs[index - 1] + yOffset;
		var sectionHeight                         = maxYs[index] - y;
		var innerSectionHeight                    = sectionHeight * GGMapComponent.innerSectionHeightRatio;
		var innerYOffset                          = (sectionHeight - innerSectionHeight) / 2.0f;
		var innerBounds                           = new Rect(-usableMapWidth / 2.0f, y + innerYOffset, usableMapWidth, innerSectionHeight);
		var selectedFrequencyPoint                = (float)random.NextDouble() * totalFrequency;
		var currentFrequencyPoint                 = 0.0f;
		GGPlatformSizeClass[] selectedSizeClasses = null;
		
		foreach (var arrangement in arrangements) {
			var nextFrequencyPoint = currentFrequencyPoint + arrangement.frequency;
			
			if (nextFrequencyPoint > selectedFrequencyPoint) {
				selectedSizeClasses = arrangement.sizeClasses;
				break;
			}
			
			currentFrequencyPoint = nextFrequencyPoint;
		}
		
		var platformsBounds      = new Rect[selectedSizeClasses.Length];
		var totalSizeClassWeight = 0.0f;
		
		for (var i = 0; i < selectedSizeClasses.Length; i += 1) {
			var sizeClass                 = selectedSizeClasses[i];
			var newIndex                  = random.Next(i, selectedSizeClasses.Length);
			selectedSizeClasses[i]        = selectedSizeClasses[newIndex];
			selectedSizeClasses[newIndex] = sizeClass;
			totalSizeClassWeight         += sizeClass.GetWeight();
		}
		
		var minHorizontalSpacing = GGMapComponent.minHorizontalPlatformSpacing;
		var boundsX              = innerBounds.x;
		
		for (var i = 0; i < selectedSizeClasses.Length; i += 1) {
			var bounds   = innerBounds;
			bounds.x     = boundsX;
			bounds.width = innerBounds.width * (selectedSizeClasses[i].GetWeight() / totalSizeClassWeight);
			
			if (i < selectedSizeClasses.Length - 1) {
				bounds.width -= minHorizontalSpacing;
			}
			
			platformsBounds[i] = bounds;
			boundsX            = bounds.xMax + minHorizontalSpacing;
		}
		
		for (var i = 0; i < selectedSizeClasses.Length; i += 1) {
			List<GameObject> platformPrototypes = null;
			
			switch (selectedSizeClasses[i]) {
				case GGPlatformSizeClass.Small:  platformPrototypes = this.smallPlatformPrototypes;  break;
				case GGPlatformSizeClass.Medium: platformPrototypes = this.mediumPlatformPrototypes; break;
				case GGPlatformSizeClass.Large:  platformPrototypes = this.largePlatformPrototypes;  break;
				default:                         Debug.LogError("Unhandled platform size class.");   break;
			}
			
			// TODO: We need to make sure not to use the same arrangement twice in a row. Would also
			// be good to prevent usage of the same platforms within the same map.
			
			var platformIndex                = random.Next(0, platformPrototypes.Count);
			var platform                     = GameObject.Instantiate(platformPrototypes[platformIndex]) as GameObject;
			var platformComponent            = platform.GetComponent<GGPlatformComponent>();
			var platformSize                 = platformComponent.size;
			var usableBounds                 = platformsBounds[i];
			usableBounds.x                  += platformSize.x / 2.0f;
			usableBounds.y                  += platformSize.y / 2.0f;
			usableBounds.width              -= platformSize.x;
			usableBounds.height             -= platformSize.y;
			var platformX                    = usableBounds.x + (float)random.NextDouble() * usableBounds.width;
			var platformY                    = usableBounds.y + (float)random.NextDouble() * usableBounds.height;
			platform.transform.localPosition = new Vector3(platformX, platformY, 0.0f);
			platform.name                    = "Platform";
			
			if (random.NextDouble() < 0.5) {
				platform.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
			}
			
			platform.SetActive(true);
			this.platformComponents.Add(platformComponent);
		}
	}
	
	private GGPlatformArrangement[] PlatformArrangementsForMapIndex(int index) {
		if (index >= 3331) {
			return GGMapComponent.map3331PlatformArrangements;
		}
		else if (index >= 1024) {
			return GGMapComponent.map1024PlatformArrangements;
		}
		else if (index >= 588) {
			return GGMapComponent.map588PlatformArrangements;
		}
		else if (index >= 259) {
			return GGMapComponent.map259PlatformArrangements;
		}
		else if (index >= 193) {
			return GGMapComponent.map193PlatformArrangements;
		}
		else if (index >= 77) {
			return GGMapComponent.map77PlatformArrangements;
		}
		
		return GGMapComponent.map0PlatformArrangements;
	}
	
	/* Loading game objects. */
	
	public void LoadPlatformPrototype(GameObject platform) {
		var platformComponent = platform.GetComponent<GGPlatformComponent>();
		
		switch (platformComponent.sizeClass) {
			case GGPlatformSizeClass.Small:  this.smallPlatformPrototypes.Add(platform);       break;
			case GGPlatformSizeClass.Medium: this.mediumPlatformPrototypes.Add(platform);      break;
			case GGPlatformSizeClass.Large:  this.largePlatformPrototypes.Add(platform);       break;
			default:                         Debug.LogError("Unhandled platform size class."); break;
		}
		
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
	
	// The amount of padding at the top of the map, which prevents platforms from being placed too
	// close to the top of the screen.
	public const float topMapPadding = 4.0f;
	
	// The ratio of the height of the inner bounds of a platform section to the full height of the
	// section. If this value is 1.0, the inner section will be just as tall as the outer section,
	// which means platforms from each section will be able to touch each other. Reducing the ratio
	// ensures that there will be more space between the platforms of each section, but reduces the
	// variety in vertical positioning of platforms within a section.
	public const float innerSectionHeightRatio = 0.85f;
	
	// The minimum amount of horizontal space required between platforms in the same section.
	public const float minHorizontalPlatformSpacing = 0.5f;
	
	// The minimum and maximum number of platform sections that can be used for an individual map.
	public const int minSectionCount = 3;
	public const int maxSectionCount = 6;
	
	// The arrangements of platform size classes that are allowed to be used in a single section
	// starting at map 0.
	public static GGPlatformArrangement[] map0PlatformArrangements = new [] {
		new GGPlatformArrangement(0.9f, GGPlatformSizeClass.Large),
		new GGPlatformArrangement(0.7f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.8f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.95f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(1.0f,  GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.75f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(1.0f,  GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.85f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small)
	};
	
	// The arrangements of platform size classes that are allowed to be used in a single section
	// starting at map 77.
	public static GGPlatformArrangement[] map77PlatformArrangements = new [] {
		new GGPlatformArrangement(0.7f, GGPlatformSizeClass.Large),
		new GGPlatformArrangement(0.5f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.6f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.65f, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.75f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(1.0f,  GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.65f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.2f, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(1.1f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.9f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small)
	};
	
	// The arrangements of platform size classes that are allowed to be used in a single section
	// starting at map 193.
	public static GGPlatformArrangement[] map193PlatformArrangements = new [] {
		new GGPlatformArrangement(0.5f, GGPlatformSizeClass.Large),
		new GGPlatformArrangement(0.3f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.4f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.85f, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.7f,  GGPlatformSizeClass.Medium, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.85f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.5f,  GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.5f, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(1.2f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.9f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small)
	};
	
	// The arrangements of platform size classes that are allowed to be used in a single section
	// starting at map 259.
	public static GGPlatformArrangement[] map259PlatformArrangements = new [] {
		new GGPlatformArrangement(0.4f, GGPlatformSizeClass.Large),
		new GGPlatformArrangement(0.1f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.3f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.9f,  GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.4f,  GGPlatformSizeClass.Medium, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.65f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.4f,  GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.6f,  GGPlatformSizeClass.Small),
		new GGPlatformArrangement(1.25f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.95f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small)
	};
	
	// The arrangements of platform size classes that are allowed to be used in a single section
	// starting at map 588.
	public static GGPlatformArrangement[] map588PlatformArrangements = new [] {
		new GGPlatformArrangement(0.3f,  GGPlatformSizeClass.Large),
		new GGPlatformArrangement(0.05f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.2f,  GGPlatformSizeClass.Large, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.8f,  GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.35f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.6f,  GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.3f,  GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.7f,  GGPlatformSizeClass.Small),
		new GGPlatformArrangement(1.25f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.95f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small)
	};
	
	// The arrangements of platform size classes that are allowed to be used in a single section
	// starting at map 1024.
	public static GGPlatformArrangement[] map1024PlatformArrangements = new [] {
		new GGPlatformArrangement(0.2f, GGPlatformSizeClass.Large),
		new GGPlatformArrangement(0.1f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.8f,  GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.25f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.35f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.2f,  GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.75f, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(1.25f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.95f, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small)
	};
	
	// The arrangements of platform size classes that are allowed to be used in a single section
	// starting at map 3331.
	public static GGPlatformArrangement[] map3331PlatformArrangements = new [] {
		new GGPlatformArrangement(0.1f, GGPlatformSizeClass.Large),
		new GGPlatformArrangement(0.01f, GGPlatformSizeClass.Large, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.6f, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.1f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Medium),
		new GGPlatformArrangement(0.2f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.1f, GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		
		new GGPlatformArrangement(0.75f, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(1.0f,  GGPlatformSizeClass.Small, GGPlatformSizeClass.Small),
		new GGPlatformArrangement(0.5f,  GGPlatformSizeClass.Small, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small)
	};
	
	public static GGPlatformSizeClass[][] standardSizeClassArrangements = new [] {
		new [] { GGPlatformSizeClass.Large                             },
		new [] { GGPlatformSizeClass.Large, GGPlatformSizeClass.Medium },
		new [] { GGPlatformSizeClass.Large, GGPlatformSizeClass.Small  },
		
		new [] { GGPlatformSizeClass.Medium, GGPlatformSizeClass.Medium                           },
		new [] { GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small                            },
		new [] { GGPlatformSizeClass.Medium, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small },
		
		new [] { GGPlatformSizeClass.Small, GGPlatformSizeClass.Small                            },
		new [] { GGPlatformSizeClass.Small, GGPlatformSizeClass.Small, GGPlatformSizeClass.Small }
	};
}
