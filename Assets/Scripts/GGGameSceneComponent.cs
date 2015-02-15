//
// The component that manages the game scene.
//

using System.Collections;
using UnityEngine;

public class GGGameSceneComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Awake() {
		this.LoadGameObjects();
	}
	
	private void LoadGameObjects() {
		var transform  = this.transform;
		var childCount = transform.childCount;
		
		for (var i = 0; i < childCount; i += 1) {
			var childTransform = transform.GetChild(i);
			var child          = childTransform.gameObject;
			var name           = child.name;
			
			switch (name) {
				case "Ball":  this.ball  = child; break;
				case "Arrow": this.arrow = child; break;
			}
		}
	}
	
	/* Accessing the component. */
	
	public static GGGameSceneComponent instance { get {
		if (_instance == null) {
			_instance = GameObject.FindObjectOfType<GGGameSceneComponent>();
		}
		
		return _instance;
	} }
	
	private static GGGameSceneComponent _instance;
	
	/* Accessing game objects and components. */
	
	// The ball object.
	[HideInInspector]
	public GameObject ball;
	
	// The arrow object.
	[HideInInspector]
	public GameObject arrow;
}
