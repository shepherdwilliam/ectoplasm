Shader "Custom/Tiling" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_TileScale ("TileScale", Range(0.1, 10)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _TileScale;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed factorX = abs(dot(IN.worldNormal, float3(1,0,0)));  
            fixed factorY = abs(dot(IN.worldNormal, float3(0,1,0)));  
            fixed factorZ = abs(dot(IN.worldNormal, float3(0,0,1)));  

            float3 scaledWorldPos = IN.worldPos / _TileScale;

            fixed4 maincx = tex2D (_MainTex, float2(scaledWorldPos.z, scaledWorldPos.y)) * factorX;
            fixed4 maincy = tex2D (_MainTex, float2(scaledWorldPos.x, scaledWorldPos.z)) * factorY;
            fixed4 maincz = tex2D (_MainTex, float2(scaledWorldPos.x, scaledWorldPos.y)) * factorZ;

            fixed4 c = (maincx + maincy + maincz) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
