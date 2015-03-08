//
// An enum that lists the possible size classes that a wall can have.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GGWallSizeClass {
	Small, Medium, Large
}

public static class GGWallSizeClassExtensions {
	public static float GetHeight(this GGWallSizeClass sizeClass) {
		switch (sizeClass) {
			case GGWallSizeClass.Small:  return 3.0f;
			case GGWallSizeClass.Medium: return 6.0f;
			case GGWallSizeClass.Large:  return 12.0f;
		}
		
		return 0.0f;
	}
}
