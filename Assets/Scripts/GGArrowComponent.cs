//
// The component that manages the rendering of the input arrow.
//

using System.Collections;
using UnityEngine;

public class GGArrowComponent: MonoBehaviour {
	/* Initializing. */
	
	public void Start() {
		this.head    = this.transform.Find("Head").gameObject;
		this.body    = this.transform.Find("Body").gameObject;
		this.isFaded = false;
		this.GenerateHeadMesh();
		this.GenerateBodyMesh();
	}
	
	/* Configuring the component. */
	
	public bool isFaded {
		get { return _isFaded; }
		set {
			_isFaded                          = value;
			this.head.renderer.material.color = _isFaded ? GGArrowComponent.fadedColor : GGArrowComponent.standardColor;
			this.body.renderer.material.color = _isFaded ? GGArrowComponent.fadedColor : GGArrowComponent.standardColor;
		}
	}
	
	private bool _isFaded = false;
	
	/* Generating meshes. */
	
	private void GenerateHeadMesh() {
		var mesh   = new Mesh();
		mesh.name  = "Arrow Head Mesh";
		var width  = GGArrowComponent.headWidth;
		var height = GGArrowComponent.headHeight;
		
		mesh.vertices = new [] {
			new Vector3(0.0f,          0.0f,    0.0f),
			new Vector3(-width / 2.0f, -height, 0.0f),
			new Vector3( width / 2.0f, -height, 0.0f)
		};
		
		mesh.triangles = new [] {
			2, 1, 0
		};
		
		mesh.RecalculateNormals();
		this.head.GetComponent<MeshFilter>().mesh = mesh;
	}
	
	private void GenerateBodyMesh() {
		var mesh      = new Mesh();
		mesh.name     = "Arrow Body Mesh";
		var thickness = GGArrowComponent.bodyThickness;
		
		mesh.vertices = new [] {
			new Vector3(-thickness / 2.0f, 1.0f, 0.0f),
			new Vector3(-thickness / 2.0f, 0.0f, 0.0f),
			new Vector3( thickness / 2.0f, 0.0f, 0.0f),
			new Vector3( thickness / 2.0f, 1.0f, 0.0f)
		};
		
		mesh.triangles = new [] {
			3, 2, 1,
			1, 0, 3
		};
		
		mesh.RecalculateNormals();
		this.body.GetComponent<MeshFilter>().mesh = mesh;
	}
	
	/* Positioning the arrow. */
	
	public void SetPosition(Vector2 origin, Vector2 tip) {
		var angle                            = Mathf.Atan2(tip.y - origin.y, tip.x - origin.x) * Mathf.Rad2Deg - 90.0f;
		var bodyLength                       = Mathf.Max(0.0f, (tip - origin).magnitude - GGArrowComponent.headHeight);
		this.head.transform.localPosition    = tip;
		this.body.transform.localPosition    = origin;
		this.head.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);
		this.body.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);
		this.body.transform.localScale       = new Vector3(1.0f, bodyLength, 1.0f);
	}
	
	/* Accessing objects and components. */
	
	// The individual head and body objects.
	private GameObject head;
	private GameObject body;
	
	/* Getting configuration values. */
	
	public static Color standardColor = new Color(0.29f, 0.53f, 0.92f, 1.0f);
	public static Color fadedColor    = new Color(0.51f, 0.75f, 0.92f, 1.0f);
	public const  float headWidth     = 0.6f;
	public const  float headHeight    = 0.475f;
	public const  float bodyThickness = 0.115f;
}
