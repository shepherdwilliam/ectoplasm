Shader "Custom/Ghost" {
	Properties{
		_Color ("Color", Color) = (1, 0.5, 0.5, 1)
		_RimColor ("Rim Color", Color) = (1,0.65,0.5,1)
		_MainTex("Texture", 2D) = "white"{}
	}

	SubShader {
		Tags {
			"Queue"      = "Transparent"
			"RenderType" = "Transparent"
		}
		LOD 200

		Pass {
  		  ZWrite ON
  		  ColorMask 0
		}

		CGPROGRAM
			#pragma surface surf Standard alpha:fade

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			struct Input {
				float3 worldNormal;
      			float3 viewDir;
				float2 uv_MainTex;
			};

			fixed4 _Color;
			fixed4 _RimColor;
			sampler2D _MainTex;

			void surf (Input IN, inout SurfaceOutputStandard o) {
				o.Albedo = _Color;
				o.Metallic = 0.1;
				o.Smoothness = 0.6;

				float alpha = saturate(1 - 0.2 *(abs(dot(IN.viewDir, IN.worldNormal))) + 0.7 * tex2D(_MainTex, IN.uv_MainTex).x);
     			o.Alpha = alpha;

				fixed4 rimColor  = _RimColor;
				float rim = saturate(1 - 0.7 * saturate(dot(IN.viewDir, o.Normal)) + 0.7 * tex2D(_MainTex, IN.uv_MainTex).x);
     			o.Emission = rimColor * rim * rim;
			}
		ENDCG
	}
	FallBack "Transparent/Diffuse"
}
