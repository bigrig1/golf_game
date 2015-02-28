//
// An enum that lists the possible size classes that a platform can be given.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GGPlatformSizeClass {
	Small, Medium, Large
}

public static class GGPlatformSizeClassExtensions {
	public static float GetWeight(this GGPlatformSizeClass sizeClass) {
		switch (sizeClass) {
			case GGPlatformSizeClass.Small:  return 1.0f;
			case GGPlatformSizeClass.Medium: return 2.0f;
			case GGPlatformSizeClass.Large:  return 3.0f;
		}
		
		return 1.0f;
	}
}
