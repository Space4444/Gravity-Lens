// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/StarShader"
{
	Properties
	{
		DiffuseMap("Texture", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
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
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D DiffuseMap;

			fixed4 frag(v2f i) : SV_Target
			{
				i.uv -= 0.5;
				float c = sqrt(dot(i.uv, i.uv));
				float angle1 = asin(min(2.4 * c, 1.0));
				float angle2 = atan2(i.uv.y, i.uv.x);
				float2 result = angle1 / 3.14159265358979 * float2(cos(angle2), sin(angle2)) + 0.5;
				return fixed4(tex2D(DiffuseMap, result).rgb, pow((0.08333333333333 - max(0,c - 0.41666666666666)) * 12.0,2.0));
			}
			ENDCG
		}
	}
}