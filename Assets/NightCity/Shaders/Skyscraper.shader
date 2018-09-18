Shader "Hidden/Skyscraper"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

        CGINCLUDE
        #ifdef ON_RENDER_SCENE_VIEW
            #define IS_SCENE_VIEW 0
        #else
            #define IS_SCENE_VIEW 1
        #endif
        ENDCG

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
            #pragma multi_compile _ ON_RENDER_SCENE_VIEW
			#pragma target 5.0

            struct procedural_data
            {
                uint id;
                uint index;
                uint verts;
                uint range;
            };
			struct geom_data
			{
				float3 center;
				float3 size;
				float3 uvRange;
				uint buildType;
			};
            struct frag_data
            {
                float4 color;
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

			#include "./Libs/Random.cginc"
			#include "./Geometries/Cube.cginc"
			#include "./Geometries/Rounded.cginc"
            #include "./Geometries/Quad.cginc"

            uniform StructuredBuffer<procedural_data> _ProceduralData;
			uniform StructuredBuffer<geom_data> _GeomData;
			uniform StructuredBuffer<uint> _RandSeeds;
            uniform StructuredBuffer<frag_data> _FragData;
			uniform sampler2D _WindowTex;

            uniform int _SeedStep;

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

                procedural_data d = _ProceduralData[inst];
                if (id >= d.verts) 
                {
                    return;
                }

                for (uint i = 0; i < d.range; i++)
                {
                    geom_data gd = _GeomData[d.index + i];
                    uint seed = _RandSeeds[d.index];

                    if (gd.buildType == 0)
                    {
                        AppendCube(gd.center, gd.size, gd.uvRange, seed, d.id, outStream);
                    }
                    else if (gd.buildType == 1)
                    {
                        uint2 seeds = uint2(seed, _RandSeeds[_SeedStep * d.index]);
                        AppendRounded(gd.center, gd.size, gd.uvRange, id, seeds, d.id, outStream);
                    }
                }
			}

            float4 frag(g2f i) : COLOR
			{
                float4 col = tex2D(_WindowTex, i.uv.xy) * (i.uv.z < 0 ? 1.0 : _FragData[round(i.uv.z)].color);
				return lerp(col, float4(0.0, 0.0, 0.0, 1.0), saturate(-1.0 * i.uv.w / 750.0) * IS_SCENE_VIEW);
			}

			ENDCG
		}
	}
}
