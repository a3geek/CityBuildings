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
                float3 v0 = float3(0.0, 0.0, 0.0);
                float3 v1 = float3(0.5, 1.0, 0.0);
                float3 v2 = float3(1.0, 0.0, 0.0);
                
                float3 uv = float3(i.uv.xy, 0.0);
                float c1 = cross(v1 - v0, uv - v0).z;
                float c2 = cross(v2 - v1, uv - v1).z;
                float c3 = cross(v0 - v2, uv - v2).z;

                bool isDown = c1 < 0.0 && c2 < 0.0 && c3 < 0.0;
                if (c1 == 0.0 || c2 == 0.0 || c3 == 0.0)
                {
                    return 0.5;
                }
                if (isDown == false)
                {
                    return 0.0;
                }

                return (i.uv.y <= _Height ? lerp(_BottomColor, _TopColor, i.uv.y) : float4(0.5, 0.5, 0.5, 1.0));
            }
            ENDCG
        }
    }
}
