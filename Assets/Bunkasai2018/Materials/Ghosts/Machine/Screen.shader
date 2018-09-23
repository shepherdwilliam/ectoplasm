Shader "Custom/Screen" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Speed ("Speed", Range(1,10)) = 1
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
		};

		fixed4 _Color;
		half _Speed;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed2 uv = IN.uv_MainTex;
			//uv.x += 0.1 * _Time;
			uv.x += _Time.y * _Speed;
			o.Albedo = tex2D (_MainTex, uv) * _Color;
			o.Metallic = 0;
			o.Smoothness = 0.5;
			o.Emission = tex2D (_MainTex, uv) / 2;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
