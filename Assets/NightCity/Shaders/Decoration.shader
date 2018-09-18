Shader "Unlit/Decoration"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile _ ON_RENDER_SCENE_VIEW
            #pragma target 5.0

            struct data
            {
                 float3 center;
                 float3 size;
                 uint buildType;
                 float height;
            };
            struct times
            {
                float time;
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

            #include "./Geometries/Sphere.cginc"
            #include "./Geometries/TriangularPyramid.cginc"

            uniform StructuredBuffer<data> _data;
            uniform StructuredBuffer<times> _times;
            uniform float _radius;

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

                data d = _data[inst];
                
                if (d.buildType == 0)
                {
                    int2 dir = _TriangularPyramidEveryDirection[id] * (d.size.xz * 0.5 - float2(_radius, _radius));
                    float3 p = d.center + float3(dir.x, d.size.y + d.height + _radius * 0.5, dir.y);

                    AppendSphere(p, _radius, -1 * ((int)inst + 1), outStream);
                }
                else if (d.buildType == 1)
                {
                    float3 p = d.center + float3(0.0, d.size.y + d.height + _radius * 0.5, 0.0);

                    if (id == 0) {
                        AppendSphere(p, _radius, -1 * ((int)inst + 1), outStream);
                    }
                    else if (id == 1)
                    {
                        p.y -= (d.height + _radius * 0.5);
                        AppendTriangularPyramid(p, float3(_radius, d.height, _radius), outStream);
                    }
                }
            }

            float4 frag(g2f i) : COLOR
            {
                int index = round(i.uv.z);
                return lerp(float4(0.0, 0.0, 0.0, 1.0), float4(1.0, 0.0, 0.0, 1.0),
                    index >= 0 ? 0.0 : _times[abs(index) - 1].time);
            }

            ENDCG
        }
    }
}
