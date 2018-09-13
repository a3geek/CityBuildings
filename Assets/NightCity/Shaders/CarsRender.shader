Shader "Hidden/CarsRender"
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
            };

            struct v2g
            {
                uint2 id : ANY;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            #include "./Geometries/Quad.cginc"

            uniform uint _maxPointPerGeom;
            uniform float _basicWidth;
            uniform float _height;
            uniform float _Size;
            uniform float4 _Color;
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

                data d = _geomData[inst];

                float hw = 0.5 * d.width;
                float2 dir = normalize(d.to - d.from);

                float2 from = d.from + dir * d.fromOffset;
                float2 to = d.to - dir * d.toOffset;

                float dis = distance(from, to);
                uint count = ceil(dis / d.interval);

                float size = _Size * (d.width / _basicWidth);

                float2 center = 0.5 * (from + to);
                float2 center2 = (from + 0.5 * dir * d.interval * (count - 1));
                float2 diff = center - center2;

                for (uint i = id * _maxPointPerGeom; i < count && i < (id + 1) * _maxPointPerGeom; i++)
                {
                    float2 v = from + min(d.interval * i, dis) * dir + diff;
                    float2 n = float2(-dir.y, dir.x);

                    AppendQuad(v + n * (hw - 0.5 * size), size, _height, outStream);
                    AppendQuad(v - n * (hw - 0.5 * size), size, _height, outStream);
                }
            }

            float4 frag(g2f i) : COLOR
            {
                float dis = distance(i.uv, float2(0.5, 0.5));
                float vdis = saturate(1.0 - dis);

                return float4((_Color * vdis).rgb, saturate(0.5 - dis));
            }

            ENDCG
        }
    }
}
