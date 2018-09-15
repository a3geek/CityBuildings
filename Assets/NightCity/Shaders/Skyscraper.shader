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

			struct data
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

			uniform StructuredBuffer<data> _GeomData;
			uniform StructuredBuffer<uint> _RandSeeds;
            uniform StructuredBuffer<frag_data> _FragData;
			uniform sampler2D _WindowTex;

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

				uint seed = _RandSeeds[2 * inst + id];
				data d = _GeomData[inst];

				if (d.buildType == 0)
				{
					if (id > 0)
					{
						return;
					}

					AppendCube(d.center, d.size, d.uvRange, seed, inst, outStream);
				}
				else if (d.buildType == 1)
				{
					uint2 seeds = uint2(seed, _RandSeeds[2 * inst]);
					AppendRounded(d.center, d.size, d.uvRange, id, seeds, inst, outStream);
				}
			}

            float4 frag(g2f i) : COLOR
			{
                float4 col = tex2D(_WindowTex, i.uv.xy) * (i.uv.z < 0 ? 1.0 : _FragData[(int)i.uv.z].color);
				return lerp(col, float4(0.0, 0.0, 0.0, 1.0), saturate(-1.0 * i.uv.w / 750.0) * IS_SCENE_VIEW);
			}

			ENDCG
		}
	}
}
