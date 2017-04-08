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

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float2 _Offset, _Scale, _Size;
			float _Rotation; // Radians ccw

			v2f vert (appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				// Calculate the sine and cosine of the angle
				float c = cos(_Rotation), s = sin(_Rotation);
				float2x2
				// Calculate the rotation matrix
				m_rotation = float2x2(
					float2(c, s),
					float2(-s, c)
				),
				// Calculate the scale matrix
				m_scale = float2x2(
					float2(1.0 / _Scale.x, 0),
					float2(0, 1.0 / _Scale.y)
				),
				// Calculate the pixel-space normalization matrix
				m_normalize = float2x2(
					float2(_Size.x, 0.0),
					float2(0.0, _Size.y)
				);
				// Offset the UV's to be centred at the origin
				o.uv = v.uv - float2(0.5, 0.5);
				// Normalize the UV coordinates from viewport space [0, 1] to pixel space [0, w/h]
				o.uv = mul(m_normalize, o.uv);
				// Rotate the UV coordinate
				o.uv = mul(m_rotation, o.uv);
				// Scale the coordinate
				o.uv = mul(m_scale, o.uv);
				// Reset the offset
				o.uv += _Size / _Scale * (0.5 - _Offset / _Size);
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target {
				// Clamp the texture
				if (i.uv.x < 0.0 || i.uv.x > 1.0 || i.uv.y < 0.0 || i.uv.y > 1.0) return fixed4(0.0, 0.0, 0.0, 0.0);
				// Sample
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
