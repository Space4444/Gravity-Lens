// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Gravitation Lensing Shader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader{
		Pass{ 
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
	uniform float2 _Position, _Position1;
	uniform float _Rad, _Rad1;
	uniform float _Ratio;
	uniform float _Distance, _Distance1;

	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		return o;
	}

	float4 frag(v2f i) : COLOR
	{
		float2 offset = i.uv - _Position; //Сдвигаем наш пиксель на нужную позицию
		float2 offset1 = i.uv - _Position1; //Сдвигаем наш пиксель на нужную позицию
		float2 ratio = { _Ratio,1.0 }; //определяем соотношение сторон экрана
		float rad = length(offset / ratio); //определяем расстояние от условного "центра" экрана.
		float rad1 = length(offset1 / ratio); //определяем расстояние от условного "центра" экрана.

		float deformation = -4.47 / pow(rad*sqrt(_Distance),2.0)*_Rad;
		float deformation1 = -4.47 / pow(rad1*sqrt(_Distance1), 2.0)*_Rad1;

		offset *= (1.0 - deformation);
		offset1 *= (1.0 - deformation1);

		offset += _Position;
		offset1 += _Position1;

		half4 res = tex2D(_MainTex, (offset + offset1)*0.5);
		float redBias = (deformation+deformation1)*0.5;
		res.r -= redBias;
		res.gb += redBias;
		if (rad < sqrt(2.0 * _Rad / _Distance) || rad1 < sqrt(2.0 * _Rad1 / _Distance1)) res.rgb = 0.0;//{res.g+=0.2;} // проверка соблюдения радиуса эйнштейна
		//if (rad*_Distance<_Rad){res.r=0;res.g=0;res.b=0;} //проверка радиуса ЧД
		return res;
	}
		ENDCG

	}
	}

		Fallback off

}