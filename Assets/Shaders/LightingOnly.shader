Shader "Lighting Only" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		[HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
	}

		SubShader{
			Blend SrcAlpha One
			ZWrite Off
			Tags {Queue = Transparent}
			ColorMask RGB
		// Vertex lights
		Pass {
			Tags {"LightMode" = "Vertex"}
			Lighting On
			
			Material {
				Diffuse[_Color]
			}
			SetTexture[_MainTex] {
				constantColor[_Color]
				Combine texture * primary DOUBLE, texture * constant
			}
		}
	}

		Fallback "VertexLit", 2

}