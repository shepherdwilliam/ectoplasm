Shader "Custom/Liquid" {
	Properties {
		_Color ("Color", Color) = (1,0,0,0.25)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}

	SubShader {
		Tags {
			"Queue"      = "Transparent"
			"RenderType" = "Transparent"
		}

		LOD 200

		CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard alpha

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
				float3 worldNormal;
      			float3 viewDir;
			};

			fixed4 _Color;

			void surf (Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Metallic = 0;
				o.Smoothness = 0;
				o.Alpha = c.a;
				float rim = saturate(dot(IN.viewDir, o.Normal));
				o.Emission = _Color * rim  / 3 ;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
