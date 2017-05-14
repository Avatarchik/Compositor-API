//
//	Compositor
//	Copyright (c) 2017 Yusuf Olokoba
//

Shader "Compositor/RenderCompositorDebug" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale", Vector) = (1.0, 1.0, 0.0, 0.0)
		_Size ("Size", Vector) = (1.0, 1.0, 0.0, 0.0)
		_Rotation ("Rotation", float) = 0.0
		_Offset ("Offset", Vector) = (0.0, 0.0, 0.0, 0.0)
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

			uniform float2 _Offset, _Scale, _Size;
			uniform float _Rotation; // Radians ccw

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				// Calculate the scale matrix
				float2x2 m_scale = float2x2 ( // CONST
					float2 (_Size.x / _Scale.x, 0.0),
					float2 (0.0, _Size.y / _Scale.y)
				);
				// Scale the layer to pixel space
				o.uv = mul(m_scale, v.uv);
				// Offset
				o.uv -= _Offset / _Scale;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target {
				// Calculate the UV
				bool wider = abs(_Scale.x) > abs(_Scale.y);
				float2
				window = float2 (wider ? 1.0 : 0.5 + 0.5 * _Size.y / _Size.x, wider ? 0.5 + 0.5 * _Size.x / _Size.y : 1.0), // CONST
				uv = float2 (i.uv.x % window.x, i.uv.y % window.y);
				// Window the UV
				if (i.uv.x > window.x || i.uv.y > window.y) return fixed4(0.0, 0.0, 0.0, 0.0);
				// Calculate the transformation matrices
				float s, c; sincos(_Rotation, s, c); // CONST
				float2x2
				m_rotation = float2x2 (
					float2 (c, s),
					float2 (-s, c)
				),
				m_scale = float2x2 (
					float2 (_Size.x, 0.0),
					float2 (0.0, _Size.y)
				),
				m_inverse = float2x2 (
					float2 (1.0 / _Size.x, 0.0),
					float2 (0.0, 1.0 / _Size.y)
				);
				// Offset the UV's to be centred at the origin
				uv -= 0.5;
				// Scale to composite space
				uv = mul(m_scale, uv);
				// Rotate the UV
				uv = mul(m_rotation, uv);
				// Scale back to normalized space
				uv = mul(m_inverse, uv);
				// Reset the offset
				uv += 0.5;
				// Clamp the texture
				if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return fixed4(0.0, 0.0, 0.0, 0.0);
				// Sample
				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
}
