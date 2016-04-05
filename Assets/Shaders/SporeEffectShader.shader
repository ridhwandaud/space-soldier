Shader "Hidden/SporeEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CameraWidth ("Camera Width", Float) = 1
		// Increase time multiplier to make the waves move more quickly.
		_TimeMultiplier ("Time multiplier", Float) = 50
		_Amplitude ("Amplitude", Float) = .012
		_WaveCountMultiplier ("Wave count multiplier", Float) = 3
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _CameraWidth;
			float _TimeMultiplier;
			float _Amplitude;
			float _WaveCountMultiplier;

			float4 frag (v2f i) : COLOR
			{
				float2 yOffset = cos((i.uv.x * _CameraWidth + _WorldSpaceCameraPos.x + _Time * _TimeMultiplier) * _WaveCountMultiplier) * _Amplitude;
				i.uv.y += yOffset;
				float num1 = (sin(_Time * 7) + 1) / 2 + .3;
				float num2 = (cos(_Time * 10) + 1) / 2 + .3;
				float num3 = (sin(_Time * 7 - 2.5) + 1) / 2 + .3;
				float4 col = tex2D(_MainTex, i.uv);
				col.r *= num1;
				col.g *= num2;
				col.b *= num3;
				return col;
			}
			ENDCG
		}
	}
}
