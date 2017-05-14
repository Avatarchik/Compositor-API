//
//	Compositor
//	Copyright (c) 2017 Yusuf Olokoba
//

Shader "Hidden/RenderCompositor" {
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

			uniform float2 _Size, _Scale, _Ratio, _Offset, _Rotation, _Window;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				// Calculate the scale matrix
				float2x2 m_scale = float2x2 (
					float2 (_Ratio.x, 0.0),
					float2 (0.0, _Ratio.y)
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
				float2 uv = float2 (i.uv.x % _Window.x, i.uv.y % _Window.y);
				// Window the UV
				if (i.uv.x > _Window.x || i.uv.y > _Window.y) return fixed4(0.0, 0.0, 0.0, 0.0);
				// Calculate the transformation matrices
				float2x2
				m_rotation = float2x2 (
					float2 (_Rotation.y, _Rotation.x),
					float2 (-_Rotation.x, _Rotation.y)
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
