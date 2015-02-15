Shader "Chromatic Labs/Flat Color" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0)
	}
	
	SubShader {
		Pass {
			CGPROGRAM
			
			#pragma vertex vertex
			#pragma fragment fragment
			
			uniform float4 _Color;
			
			struct VertexInput {
				float4 vertex : POSITION;
			};
			
			struct VertexOutput {
				float4 position : SV_POSITION;
			};
			
			VertexOutput vertex(VertexInput input) {
				VertexOutput output;
				output.position = mul(UNITY_MATRIX_MVP, input.vertex);
				return output;
			}
			
			float4 fragment(VertexOutput output) : COLOR {
				return _Color;
			}
			
			ENDCG
		}
	}
	
	Fallback "Diffuse"
}
