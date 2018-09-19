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
            uniform sampler2D _Tex;

            float4 frag(v2f_img i) : COLOR
            {
                return i.uv.y <= _Height ? tex2D(_Tex, i.uv) : 1.0;
            }
            ENDCG
        }
    }
}
