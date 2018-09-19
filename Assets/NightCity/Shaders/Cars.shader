Shader "Hidden/Cars"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 5.0

            struct data
            {
                float2 pos;
                float2 dir;
            };

            struct v2g
            {
                uint2 id : ANY;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            #include "./Geometries/Quad.cginc"
            
            uniform float _Height, _Size;
            uniform float4 _ForwardColor, _BackColor;
            uniform StructuredBuffer<data> _GeomData;
            uniform float _DofPower;
            uniform float4 _DofColor;

            float3 getCameraForward()
            {
                return -UNITY_MATRIX_V[2].xyz;
            }

            v2g vert(uint id : SV_VertexID, uint inst : SV_InstanceID)
            {
                v2g o;
                o.id = uint2(id, inst);

                return o;
            }   

            [maxvertexcount(128)]
            void geom(point v2g input[1], inout TriangleStream<g2f> outStream)
            {
                v2g v = input[0];
                uint id = v.id.x;
                uint inst = v.id.y;

                data d = _GeomData[inst];

                float view = dot(getCameraForward(), float3(d.dir.x, 0.0, d.dir.y));
                float3 forward = float3(d.dir.x, 0.0, d.dir.y);
                float3 up = float3(0.0, 1.0, 0.0);

                AppendQuad(float3(d.pos.x, _Height, d.pos.y), _Size, forward, up, view > 0 ? 1.0 : -1.0, outStream);
            }

            float4 frag(g2f i) : COLOR
            {
                float dis = distance(i.uv.xy, float2(0.5, 0.5));
                float vdis = saturate(1.0 - dis);
                
                float4 c = i.uv.z >= 0.0 ? _BackColor : _ForwardColor;
                c = float4((c * vdis).rgb, saturate(0.5 - dis));

                return lerp(c, float4(_DofColor.rgb, 0.0), saturate(-1.0 * i.uv.w / _DofPower));
            }

            ENDCG
        }
    }
}
