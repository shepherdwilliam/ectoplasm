Shader "Custom/Button" {
	Properties {
		_Color ("Color", Color) = (0,0,0,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.7
		_Metallic ("Metallic", Range(0,1)) = 0.1
		_Frequency ("Frequency", Range(1,20)) = 5
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		half _Frequency;
		fixed4 _Color;

		float random (fixed2 p) { 
            return frac(sin(dot(p, fixed2(12.9898,78.233))) * 43758.5453);
        }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			half emi = (sin(_Time.w*_Frequency) + 1.0)*.5;
			o.Emission = _Color * emi;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
