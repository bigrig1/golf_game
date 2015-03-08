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
	
	// The list of platform components that were being used in the previous map.
	[HideInInspector]
	public List<GGPlatformComponent> previousPlatformComponents = new List<GGPlatformComponent>();
	
	// The list of platform components that are currently being used in the map.
	[HideInInspector]
	public List<GGPlatformComponent> platformComponents = new List<GGPlatformComponent>();
	
	// The list of platform components that will be used for the next map.
	[HideInInspector]
	public List<GGPlatformComponent> nextPlatformComponents = new List<GGPlatformComponent>();
	
	// The list of wall components that were being used in the previous map.
	[HideInInspector]
	public List<GGWallComponent> previousWallComponents = new List<GGWallComponent>();
	
	// The list of wall components that are currently being used in the map.
	[HideInInspector]
	public List<GGWallComponent> wallComponents = new List<GGWallComponent>();
	
	// The list of wall components that will be used in the next map.
	[HideInInspector]
	public List<GGWallComponent> nextWallComponents = new List<GGWallComponent>();
	
	/* Managing map component prototypes. */
	
	// The platform components for the platform prototypes, grouped by size class.
	[HideInInspector]
	public List<GameObject> smallPlatformPrototypes  = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> mediumPlatformPrototypes = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> largePlatformPrototypes  = new List<GameObject>();
	
	// The wall components for the wall prototypes, grouped by size class.
	[HideInInspector]
	public List<GameObject> smallWallPrototypes  = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> mediumWallPrototypes = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> largeWallPrototypes  = new List<GameObject>();
	
	/* Building maps. */
	
	private float yOffset = 0.0f;
	public int currentMapIndex { get; private set; }
	private bool shouldCreateDebugObjects = false;
	
	// Builds the first map of the game. An initial map index can be passed in to start at an
	// arbitrary map.
	public void BuildFirstMap(int initialMapIndex) {
		this.currentMapIndex = initialMapIndex;
		this.BuildMap(this.currentMapIndex, false);
		this.yOffset += GGMapComponent.mapHeight;
		this.BuildMap(this.currentMapIndex + 1, true);
	}
	
	// Builds the next map.
	public void BuildNextMap() {
		this.DestroyPreviousMap();
		
		foreach (var wallComponent in this.wallComponents) {
			this.previousWallComponents.Add(wallComponent);
		}
		
		foreach (var platformComponent in this.platformComponents) {
			this.previousPlatformComponents.Add(platformComponent);
		}
		
		this.wallComponents.Clear();
		this.platformComponents.Clear();
		
		foreach (var wallComponent in this.nextWallComponents) {
			this.wallComponents.Add(wallComponent);
		}
		
		foreach (var platformComponent in this.nextPlatformComponents) {
			this.platformComponents.Add(platformComponent);
			platformComponent.gameObject.SetActive(true);
		}
		
		this.nextWallComponents.Clear();
		this.nextPlatformComponents.Clear();
		
		this.currentMapIndex += 1;
		this.yOffset         += GGMapComponent.mapHeight;
		this.BuildMap(this.currentMapIndex + 1, true);
	}
	
	// Destroys everything in the previous map. This should be called after the transition to the
	// next map finishes to clean up unneeded objects.
	public void DestroyPreviousMap() {
		foreach (var wallComponent in this.previousWallComponents) {
			GameObject.Destroy(wallComponent.gameObject);
		}
		
		foreach (var platformComponent in this.previousPlatformComponents) {
			GameObject.Destroy(platformComponent.gameObject);
		}
		
		this.previousWallComponents.Clear();
		this.previousPlatformComponents.Clear();
	}
	
	// Procedurally generates a map by creating all the platforms and walls for it. You'll always
	// get the same map for a given map index. If isNextMap is true, the walls and platforms will be
	// added to the next
	private void BuildMap(int mapIndex, bool isNextMap) {
		var random = new System.Random(168403912 + mapIndex);
		this.AddWalls(isNextMap, random);
		this.AddPlatforms(mapIndex, isNextMap, random);
		this.groundComponent.gameObject.SetActive(mapIndex <= 1);
		
		if (isNextMap) {
			foreach (var platformComponent in this.nextPlatformComponents) {
				platformComponent.gameObject.SetActive(false);
			}
		}
	}
	
	// Adds all of the walls for the map.
	private void AddWalls(bool isNextMap, System.Random random) {
		var wallArrangements = GGMapComponent.wallArrangements;
		var leftArrangement  = wallArrangements[random.Next(wallArrangements.Length)];
		var rightArrangement = wallArrangements[random.Next(wallArrangements.Length)];
		var wallY            = 0.0f;
		
		for (var i = 0; i < leftArrangement.Length; i += 1) {
			var sizeClass             = leftArrangement[i];
			var newIndex              = random.Next(i, leftArrangement.Length);
			leftArrangement[i]        = leftArrangement[newIndex];
			leftArrangement[newIndex] = sizeClass;
		}
		
		for (var i = 0; i < rightArrangement.Length; i += 1) {
			var sizeClass              = rightArrangement[i];
			var newIndex               = random.Next(i, rightArrangement.Length);
			rightArrangement[i]        = rightArrangement[newIndex];
			rightArrangement[newIndex] = sizeClass;
		}
		
		foreach (var sizeClass in leftArrangement) {
			wallY += this.AddWall(wallY + this.yOffset, sizeClass, true, isNextMap, random);
		}
		
		wallY = 0.0f;
		
		foreach (var sizeClass in rightArrangement) {
			wallY += this.AddWall(wallY + this.yOffset, sizeClass, false, isNextMap, random);
		}
	}
	
	// Adds a wall segment at the given Y position and returns the height of the segment that was
	// added.
	private float AddWall(float y, GGWallSizeClass sizeClass, bool isOnLeftSide, bool isNextMap, System.Random random) {
		var x = GGMapComponent.mapWidth / 2.0f;
		
		if (isOnLeftSide) {
			x = -x;
		}
		
		List<GameObject> wallPrototypes = null;
		
		switch (sizeClass) {
			case GGWallSizeClass.Small:  wallPrototypes = this.smallWallPrototypes;        break;
			case GGWallSizeClass.Medium: wallPrototypes = this.mediumWallPrototypes;       break;
			case GGWallSizeClass.Large:  wallPrototypes = this.largeWallPrototypes;        break;
			default:                     Debug.LogError("Unhandled platform size class."); break;
		}
		
		var wallIndex                = random.Next(wallPrototypes.Count);
		var wall                     = GameObject.Instantiate(wallPrototypes[wallIndex]) as GameObject;
		var wallComponent            = wall.GetComponent<GGWallComponent>();
		var wallHeight               = wallComponent.height;
		wall.name                    = "Wall";
		wall.transform.localPosition = new Vector3(x, y + wallHeight / 2.0f, 0.0f);
		
		if (!isOnLeftSide) {
			wall.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
		}
		
		wall.SetActive(true);
		
		if (isNextMap) {
			this.nextWallComponents.Add(wallComponent);
		}
		else {
			this.wallComponents.Add(wallComponent);
		}
		
		return wallHeight;
	}
	
	// Adds all of the platforms for the map.
	private void AddPlatforms(int mapIndex, bool isNextMap, System.Random random) {
		var usableMapHeight             = GGMapComponent.mapHeight - GGMapComponent.topMapPadding;
		var platformArrangements        = this.PlatformArrangementsForMapIndex(mapIndex);
		var sectionCount                = random.Next(GGMapComponent.minSectionCount, GGMapComponent.maxSectionCount + 1);
		var sectionCountReductionChance = (float)mapIndex / 500.0f;
		
		if (sectionCount > GGMapComponent.minSectionCount && (float)random.NextDouble() < sectionCountReductionChance) {
			sectionCount -= 1;
		}
		
		var sectionMaxYs            = new float[sectionCount];
		var baseSectionHeight       = usableMapHeight / (float)sectionCount;
		var divisionAdjustmentRange = baseSectionHeight / 6.0f;
		var totalPlatformFrequency  = 0.0f;
		
		foreach (var platformArrangement in platformArrangements) {
			totalPlatformFrequency += platformArrangement.frequency;
		}
		
		for (var i = 0; i < sectionCount - 1; i += 1) {
			sectionMaxYs[i] = (float)(i + 1) * baseSectionHeight + divisionAdjustmentRange + (float)(random.NextDouble() - 0.5);
		}
		
		sectionMaxYs[sectionCount - 1] = usableMapHeight;
		
		for (var i = 0; i < sectionCount; i += 1) {
			this.AddPlatformSection(i, sectionMaxYs, sectionCount, platformArrangements, totalPlatformFrequency, isNextMap, random);
		}
	}
	
	// Adds all the platforms for the given section.
	private void AddPlatformSection(int index, float[] localMaxYs, int sectionCount, GGPlatformArrangement[] arrangements, float totalFrequency, bool isNextMap, System.Random random) {
		var usableMapWidth                        = GGMapComponent.mapWidth - GGMapComponent.horizontalMapPadding * 2.0f;
		var localY                                = index == 0 ? 0.0f : localMaxYs[index - 1];
		var y                                     = localY + this.yOffset;
		var sectionHeight                         = localMaxYs[index] - localY;
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
		}
		
		foreach (var sizeClass in selectedSizeClasses) {
			totalSizeClassWeight += sizeClass.GetWeight();
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
			// be good to prevent usage of the same platforms within the same map. Actually, even
			// better I think would be to not allow the same number of platforms in a section twice
			// in a row. That should go a long way to help prevent unbeatable maps, since that
			// should cause them to be kind of staggered between rows. Might run into issues in
			// later arrangement sets though, where it falls into an obvious pattern of 1 platform -
			// 2 platforms since it'll be very rare to have 3 platforms. Maybe we'll just bias
			// towards a different platform count between adjacent sections rather than strictly
			// enforcing it.
			
			// TEMP: Right now we don't have any large pieces, so we run into trouble when we look
			// for a large piece. This won't be a problem once we have some large pieces, though.
			if (platformPrototypes.Count == 0) {
				continue;
			}
			
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
			
			if (isNextMap) {
				this.nextPlatformComponents.Add(platformComponent);
			}
			else {
				this.platformComponents.Add(platformComponent);
			}
			
			if (this.shouldCreateDebugObjects) {
				var debugObject                     = GameObject.Instantiate(Resources.Load("Prefabs/Debug Bounds")) as GameObject;
				debugObject.transform.localPosition = new Vector3(platformsBounds[i].center.x, platformsBounds[i].center.y, 0.5f);
				debugObject.transform.localScale    = new Vector3(platformsBounds[i].width, platformsBounds[i].height, 1.0f);
				debugObject.renderer.material.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
			}
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
		var wallComponent = wall.GetComponent<GGWallComponent>();
		
		switch (wallComponent.sizeClass) {
			case GGWallSizeClass.Small:  this.smallWallPrototypes.Add(wall);           break;
			case GGWallSizeClass.Medium: this.mediumWallPrototypes.Add(wall);          break;
			case GGWallSizeClass.Large:  this.largeWallPrototypes.Add(wall);           break;
			default:                     Debug.LogError("Unhandled wall size class."); break;
		}
		
		wall.SetActive(false);
	}
	
	public void LoadGround(GameObject ground) {
		this.groundComponent = ground.GetComponent<GGGroundComponent>();
		this.groundCollider  = ground.collider2D;
	}
	
	/* Getting configuration values. */
	
	// The width of each map. This defines where walls and platforms are placed.
	public const float mapWidth = 19.0f;
	
	// The full height of each map, from the bottom of the screen to the top.
	public const float mapHeight = 36.0f;
	
	// The amount of padding on the sides of the map, which is used for positioning platforms so
	// that they don't overlap the walls.
	public const float horizontalMapPadding = 2.5f;
	
	// The amount of padding at the top of the map, which prevents platforms from being placed too
	// close to the top of the screen.
	public const float topMapPadding = 4.5f;
	
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
	public const int maxSectionCount = 5;
	
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
	
	// The possible arrangements of wall pieces based on their size class. The order gets shuffled
	// when the map is generated, so there's no need to have multiple entries with the same
	// combination of size classes.
	public static GGWallSizeClass[][] wallArrangements = new [] {
		new [] { GGWallSizeClass.Large, GGWallSizeClass.Large, GGWallSizeClass.Large },
		new [] { GGWallSizeClass.Large, GGWallSizeClass.Large, GGWallSizeClass.Medium, GGWallSizeClass.Medium },
		new [] { GGWallSizeClass.Large, GGWallSizeClass.Large, GGWallSizeClass.Medium, GGWallSizeClass.Small, GGWallSizeClass.Small },
		new [] { GGWallSizeClass.Large, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Medium },
		new [] { GGWallSizeClass.Large, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Small, GGWallSizeClass.Small },
		new [] { GGWallSizeClass.Large, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Small, GGWallSizeClass.Small, GGWallSizeClass.Small, GGWallSizeClass.Small },
		new [] { GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Small, GGWallSizeClass.Small, GGWallSizeClass.Small, GGWallSizeClass.Small },
		new [] { GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Medium, GGWallSizeClass.Small, GGWallSizeClass.Small }
	};
}
