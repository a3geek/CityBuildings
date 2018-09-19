Shader "Hidden/WorldClock"
{
	Properties
	{
	}
	SubShader
	{
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _BorderColor, _ZeroColor, _ThreeColor, _SixColor, _NineColor;
            float _Radius, _Bolid, _Ring, _Offset, _Blend;

            float map(float s, float a1, float a2, float b1, float b2)
            {
                return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
            }

            float4 frag(v2f_img i) : COLOR
            {
                float d = distance(float2(0.5, 0.5), i.uv.xy);
                if (d > _Radius)
                {
                    return 0.0;
                }
                else if (d >= _Radius - _Bolid)
                {
                    return _BorderColor;
                }
                else if (d <= _Ring)
                {
                    return 0.0;
                }

                float2 dir = i.uv.xy - float2(0.5, 0.5);
                d = saturate(dot(float2(0.0, i.uv.y >= 0.5 ? 1.0 : -1.0), dir) / (1.0 * length(dir)) - _Offset);
                d = d < 0.5 - _Blend ? 0.0 : (d > 0.5 + _Blend ? 1.0 : map(d, 0.5 - _Blend, 0.5 + _Blend, 0.0, 1.0));

                return lerp(i.uv.x >= 0.5 ? _ThreeColor : _NineColor, i.uv.y >= 0.5 ? _ZeroColor : _SixColor, d);
            }
            ENDCG
        }
	}
}
