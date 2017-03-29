Shader "Hidden/Compositor2D" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Cull Off
		ZWrite Off
		Lighting Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#define PI 3.14159265359
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float2 _Offset, _Scale;
			float _Rotation;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				float c = cos(_Rotation * PI / 180.0), s = sin(_Rotation * PI / 180.0);
				float2x2
				rotation = float2x2(
					float2(c, s),
					float2(-s, c)
				), scale = float2x2(
					float2(1.0 / _Scale.x, 0),
					float2(0, 1.0 / _Scale.y)
				), transformation = mul(scale, rotation);
				o.uv = v.uv + _Offset - float2(0.5, 0.5);
				//o.uv = mul(o.uv, scale);
				o.uv = mul(o.uv, transformation);
				o.uv = o.uv + float2(0.5, 0.5);
				return o;
			}
			
			sampler2D _MainTex, _BaseTex;

			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);
				if (i.uv.x < 0.0 || i.uv.x > 1.0 || i.uv.y < 0.0 || i.uv.y > 1.0) return fixed4(0.0, 0.0, 0.0, 0.0);
				return col;
			}
			ENDCG
		}
	}
}
