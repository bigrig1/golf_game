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
			_isFaded = value;
			// TODO: Update color.
		}
	}
	
	private bool _isFaded = false;
	
	public float power {
		get { return _power; }
		set {
			_power = Mathf.Clamp01(value);
			
			var colors = GGArrowComponent.colors;
			
			if (colors.Length == 1) {
				this.SetColor(colors[0]);
			}
			else if (colors.Length > 0) {
				var rangePerColor = 1.0f / (float)(colors.Length - 1);
				var leftIndex     = (int)Mathf.Clamp(Mathf.Floor(_power / rangePerColor), 0, colors.Length - 2);
				var leftColor     = colors[leftIndex];
				var rightColor    = colors[leftIndex + 1];
				var progress      = (_power - (float)leftIndex * rangePerColor) / rangePerColor;
				
				var color = new Color(
					leftColor.r * (1.0f - progress) + rightColor.r * progress,
					leftColor.g * (1.0f - progress) + rightColor.g * progress,
					leftColor.b * (1.0f - progress) + rightColor.b * progress
				);
				
				this.SetColor(color);
			}
		}
	}
	
	private float _power = 0.0f;
	
	private void SetColor(Color color) {
		this.head.renderer.material.color = color;
		this.body.renderer.material.color = color;
	}
	
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
	
	public static Color[] colors = new [] {
		new Color(234.0f / 255.0f, 217.0f / 255.0f, 40.0f / 255.0f, 1.0f),
		new Color(236.0f / 255.0f, 150.0f / 255.0f, 62.0f / 255.0f, 1.0f),
		new Color(204.0f / 255.0f, 56.0f  / 255.0f, 26.0f / 255.0f, 1.0f)
	};
	
	public const float headWidth     = 0.6f;
	public const float headHeight    = 0.475f;
	public const float bodyThickness = 0.115f;
}
