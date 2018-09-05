Shader "Hidden/Skyscraper"
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
				float3 uv : TEXCOORD0;
			};

			#include "./Libs/Random.cginc"
			#include "./Geometries/Cube.cginc"
			#include "./Geometries/Rounded.cginc"

			uniform StructuredBuffer<data> _geomData;
			uniform StructuredBuffer<uint> _randSeeds;
            uniform StructuredBuffer<frag_data> _fragData;
			uniform sampler2D _windowTex;

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

				uint seed = _randSeeds[2 * inst + id];
				data d = _geomData[inst];

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
					uint2 seeds = uint2(seed, _randSeeds[2 * inst]);
					AppendRounded(d.center, d.size, d.uvRange, id, seeds, inst, outStream);
				}
			}

			fixed4 frag(g2f i) : COLOR
			{
				return tex2D(_windowTex, i.uv.xy) * (i.uv.z < 0 ? 1.0 : _fragData[(int)i.uv.z].color);
			}

			ENDCG
		}
	}
}
