// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "FX/SpotLight" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Color ("_Color", Color) = (1, 1, 1, 1)
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off ZWrite Off Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {

			Material
			{
				Diffuse[_Color]
			}

			Lighting on

			SetTexture [_MainTex] {
				constantColor[_Color]
				combine texture * primary , texture * constant
			}
		}
	}
}
}