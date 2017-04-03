// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FX/Shadow" {
	Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Color("Main Color", Color) = (1,1,1,0.5)
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off  ZWrite Off Fog { Color (0,0,0,0) }
	
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
