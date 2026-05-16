// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/WorldRuntime"
{
	Properties
	{
		_MainTex("texture", 2D) = "white" {}
		_BackTex("Background texture", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
		//Blend SrcAlpha OneMinusSrcAlpha
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

		sampler2D _BackTex;
		float2 pos, scrSize, size2;
		uniform float2 _Position, _Position1;
		uniform float _Rad, _Rad1;
		uniform float _Ratio;
		uniform float _Distance, _Distance1;

		fixed4 frag(v2f i) : SV_Target
		{
		float2 offset = i.uv - _Position; //Сдвигаем наш пиксель на нужную позицию
		float2 offset1 = i.uv - _Position1; //Сдвигаем наш пиксель на нужную позицию
		float2 ratio = { _Ratio,1.0 }; //определяем соотношение сторон экрана
		float rad = length(offset / ratio); //определяем расстояние от условного "центра" экрана.
		float rad1 = length(offset1 / ratio); //определяем расстояние от условного "центра" экрана.

		float deformation = 4.47 / pow(rad*sqrt(_Distance), 2.0)*_Rad;
		float deformation1 = 4.47 / pow(rad1*sqrt(_Distance1), 2.0)*_Rad1;

		offset *= 1.0 - deformation;
		offset1 *= 1.0 - deformation1;

		offset += _Position;
		offset1 += _Position1;

		i.uv = (offset + offset1)*0.5;

		half4 res = tex2D(_BackTex, (i.uv / 2048.0 * scrSize + pos / 1024.0) % 1.0);
		
		if (rad < sqrt(2.0 * _Rad / _Distance) || rad1 < sqrt(2.0 * _Rad1 / _Distance1)) res.rgb = 0.0;//{res.g+=0.2;} // проверка соблюдения радиуса эйнштейна
																									   //if (rad*_Distance<_Rad){res.r=0;res.g=0;res.b=0;} //проверка радиуса ЧД
		return res;
		}
		ENDCG
	}
	}
}