// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Q5/Unity/Unlit/Texture Alpha Control2" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Alpha ("Alpha Control", Range (0.00,1.00)) = 1.00
	_Color("Color", Color) = (1, 1, 1, 1)
	_Specular ("Specular", Color) = (1, 1, 1, 1)
	_Gloss ("Gloss", Range(8.0, 256)) = 20
}

SubShader {
	//Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
	Tags { "RenderType"="Transparent"  "Queue"="Transparent"  "LightMode"="ForwardBase"}
	//Tags { "Queue"="Transparent" }
	LOD 100
	Blend SrcAlpha OneMinusSrcAlpha 

	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD2;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				//UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Alpha;
			float4 _Color;
			fixed4 _Specular;
			float _Gloss;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * _Color * 1.5;
				//UNITY_APPLY_FOG(i.fogCoord, col);
				//UNITY_OPAQUE_ALPHA(col.a);
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * col.rbg;
				fixed3 diffuse = _LightColor0.rgb * col.rbg * max(0, dot(worldNormal, worldLightDir));

				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

				col.a = _Alpha;

				return fixed4(col.rgb + specular, _Alpha);
			}
		ENDCG
	}
}

}
