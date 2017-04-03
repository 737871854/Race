Shader "Q5/Unity/Legacy Shaders/Transparent/Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Alpha("Alpha", Range(0,1)) = 1
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

CGPROGRAM
#pragma surface surf Lambert alpha:blend

sampler2D _MainTex;
fixed4 _Color;
fixed _Alpha;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb * 5;
	o.Alpha = _Alpha;
}
ENDCG
}

Fallback "Q5/Unity/Legacy Shaders/Transparent/VertexLit"
}
