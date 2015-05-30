//
// A class that bridges calls from Unity to native platforms.
//

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GGBridge {
	public static float GetScreenPixelScale() {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return _GetScreenPixelScale();
		}
		
		return 1.0f;
	}
	
	[DllImport ("__Internal")]
	private static extern float _GetScreenPixelScale();
}
