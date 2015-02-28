//
// A model that encapsulates a particular arrangement of platforms based on their size classes. Used
// during level generation.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GGPlatformArrangement {
	// The frequency that this particular arrangement appears relative to other arrangements. The
	// standard frequency is 1.0, so an arrangement with a frequency of 2.0 would appear twice as
	// often as a standard arrangement.
	public float frequency;
	
	// The actual size classes in the arrangement.
	public GGPlatformSizeClass[] sizeClasses;
	
	public GGPlatformArrangement(float frequency, params GGPlatformSizeClass[] sizeClasses) {
		this.frequency   = frequency;
		this.sizeClasses = sizeClasses;
	}
}
