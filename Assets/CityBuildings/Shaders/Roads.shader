Shader "Hidden/Roads"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
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
                float fromOffset;
                float toOffset;
                float width;
                float interval;
                float magnitude;
                float2 direction;
                float bolid;
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

            uniform uint _MaxPointPerGeom;
            uniform float _BasicWidth, _Size, _Height;
            uniform float4 _Color, _LineColor;
            uniform StructuredBuffer<data> _GeomData;
            uniform float _DofPower;
            uniform float4 _DofColor;
            uniform int _IsNight;

            v2g vert(uint id : SV_VertexID, uint inst : SV_InstanceID)
            {
                v2g o;
                o.id = uint2(id, inst);

                return o;
            }
            
            [maxvertexcount(128)]
            void geom(point v2g input[1], inout TriangleStream<g2f> outStream)
            {
                uint id = input[0].id.x;
                uint inst = input[0].id.y;

                data d = _GeomData[inst];
    
                float hw = 0.5 * d.width;
                float2 dir = d.direction;
                float2 n = float2(-dir.y, dir.x);

                float2 from = d.from + dir * d.fromOffset;
                float2 to = d.to - dir * d.toOffset;

                float dis = d.magnitude;
                uint count = ceil(dis / d.interval);

                float size = _Size * (d.width / _BasicWidth);

                float2 center = 0.5 * (from + to);
                float2 loopCenter = (from + 0.5 * dir * d.interval * (count - 1));
                float2 diff = center - loopCenter;

                for (uint i = id * _MaxPointPerGeom; i < count && i < (id + 1) * _MaxPointPerGeom; i++)
                {
                    float2 v = from + min(d.interval * i, dis) * dir + diff;

                    if (_IsNight == 1)
                    {
                        float2 p1 = v + n * (hw - 0.5 * size);
                        float2 p2 = v - n * (hw - 0.5 * size);

                        AppendQuad(float3(p1.x, _Height, p1.y), size, float3(0.0, 1.0, 0.0), float3(0.0, 0.0, 1.0), 0.0, outStream);
                        AppendQuad(float3(p2.x, _Height, p2.y), size, float3(0.0, 1.0, 0.0), float3(0.0, 0.0, 1.0), 0.0, outStream);

                        continue;
                    }

                    if (count == 1)
                    {
                        float2 s = (to - from) * 0.5 + abs(n) * d.bolid;
                        AppendQuad(float3(v.x, 0.0, v.y), s, float3(0.0, 1.0, 0.0), float3(0.0, 0.0, 1.0), 0.0, outStream);

                        continue;
                    }
                    else if (i == count - 1)
                    {
                        break;
                    }

                    float2 v2 = from + min(d.interval * (i + 1), dis) * dir + diff;

                    float2 p = v + (v2 - v) * 0.5;
                    float2 s = (v2 - v) * 0.5 + abs(n) * d.bolid;

                    AppendQuad(float3(p.x, 0.0, p.y), s, float3(0.0, 1.0, 0.0), float3(0.0, 0.0, 1.0), 0.0, outStream);
                }
            }

            float4 frag(g2f i) : COLOR
            {
                if (_IsNight == 0)
                {
                    return _LineColor;
                }

                float dis = distance(i.uv.xy, float2(0.5, 0.5));
                float vdis = saturate(1.0 - dis);

                float4 c = float4((_Color * vdis).rgb, saturate(0.5 - dis));
                return lerp(c, float4(_DofColor.rgb, 0.0), saturate(-1.0 * i.uv.w / _DofPower));
            }

            ENDCG
        }
    }
}
