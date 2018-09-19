Shader "Hidden/BuildScaler"
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

            uniform float _Height;
            uniform float4 _TopColor, _BottomColor;
            uniform float _Bolid;

            float4 frag(v2f_img i) : COLOR
            {
                bool isBolid = i.uv.x <= _Bolid || i.uv.x >= 1.0 - _Bolid ||
                    i.uv.y <= _Bolid || i.uv.y >= 1.0 - _Bolid;

                return isBolid == true ? 1.0 : (i.uv.y <= _Height ?
                    lerp(_BottomColor, _TopColor, i.uv.y) : 1.0);
            }
            ENDCG
        }
    }
}
