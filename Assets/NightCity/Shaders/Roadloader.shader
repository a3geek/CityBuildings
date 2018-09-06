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
                float width;
                float step;
                uint count;
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
            uniform float _height;
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

                for (uint i = id * _maxPointPerGeom; i < d.count && i < (id + 1) * _maxPointPerGeom; i++)
                {
                    float2 v = d.from + (d.step * i) * dir;
                    float2 n = float2(-dir.y, dir.x);

                    AppendQuad(v + n * (hw - 1.0), 1.0, _height, outStream);
                    AppendQuad(v - n * (hw - 1.0), 1.0, _height, outStream);
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
