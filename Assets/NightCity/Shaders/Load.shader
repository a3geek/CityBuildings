Shader "Hidden/Load"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1.0, 1.0, 0.0, 1.0)
        _Radius("Radius", Float) = 0.25
        _Bolid("Bolid", Float) = 0.01

_T("T", Range(0.0, 1.0)) = 0.0
	}
	SubShader
	{
		Cull Off
        ZWrite Off
        ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
            float4 _Color;
            float _Radius, _Bolid, _T;
            uniform float _Timer;

			fixed4 frag (v2f_img i) : COLOR
			{
                float x = _ScreenParams.x;
                float y = _ScreenParams.y;

                float d = 0.0;
                if (x > y)
                {
                    d = distance(float2(0.5 * (x / y), 0.5), float2(i.uv.x * (x / y), i.uv.y));
                }
                else
                {
                    d = distance(float2(0.5, 0.5 * (y / x)), float2(i.uv.x, i.uv.y * (y / x)));
                }

                float r = _Radius - _Bolid;
                float theta = saturate(_T) * UNITY_TWO_PI;
                float zero = UNITY_PI * 0.5;

                float3 start = float3(0.0, 0.0, r);
                float3 end = float3(cos(zero - theta), 0.0, sin(zero - theta));
                float3 dir = float3(i.uv.x, 0.0, i.uv.y) - float3(0.5, 0.0, 0.5);

                float4 c = 0.0;
                if (theta > UNITY_PI)
                {
                    c = cross(start, dir).y >= 0.0 || cross(end, dir).y <= 0.0 ? _Color : 0.0;
                }
                else if(theta > 0.0)
                {
                    c = cross(start, dir).y >= 0.0 && cross(end, dir).y <= 0.0 ? _Color : 0.0;
                }

                return (d <= _Radius - _Bolid) ? c : (d > _Radius - _Bolid && d <= _Radius ? _Color : 0.0);
			}
			ENDCG
		}
	}
}
