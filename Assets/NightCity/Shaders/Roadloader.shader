Shader "Hidden/Roadloader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

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
                float2 from;
                float2 to;
                float2 normal;
                float width;
                float interval;
            };

            struct v2g
            {
                uint2 id : ANY;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float3 uv : TEXCOORD0;
            };

            uniform StructuredBuffer<data> _geomData;

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

                data d = _geomData[id];

                float dis = distance(d.to, d.from);
                float dir = normalize(d.to - d.from);
                float hw = 0.5 * d.width;

                float step = dis / d.interval;
                int count = trunc(step);

                for (int i = 0; i < count; i++)
                {
                    float2 v = d.from + (step * i) * dir;
                    
                }
            }

            fixed4 frag(g2f i) : COLOR
            {
                return 1;
                //return tex2D(_windowTex, i.uv.xy) * (i.uv.z < 0 ? 1.0 : _fragData[(int)i.uv.z].color);
            }

            ENDCG
        }
    }
}
