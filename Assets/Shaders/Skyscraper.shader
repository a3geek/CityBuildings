Shader "Unlit/Skyscraper"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "./Consts/Cube.cginc"

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			struct data
			{
				float3 center;
				float4 color;
				float3 size;
			};

			struct v2g
			{
				float4 center : SV_POSITION;
				float4 color : COLOR;
				float3 size : ANY;
			};

			struct g2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};

			uniform int _parts_count;
			StructuredBuffer<data> _data;

			v2g vert(uint id : SV_VertexID, uint inst : SV_InstanceID)
			{
				v2g o;
				data d = _data[_parts_count * inst + id];

				o.center = float4(d.center, 1.0f);
				o.color = d.color;
				o.size = d.size;

				return o;
			}

			[maxvertexcount(APPEND_VERTEX_COUNT)]
			void geom(point v2g input[1], inout TriangleStream<g2f> outStream)
			{
				v2g d = input[0];
				g2f v[VERTEX_POSITION_COUNT];

				for (int i = 0; i < VERTEX_POSITION_COUNT; i++)
				{
					float3 pos = d.center + _VertexPos[i] * d.size * 0.5f;

					v[i].color = d.color;
					v[i].pos = mul(UNITY_MATRIX_VP, float4(pos, 1.0f));
				}

				for (i = 0; i < SURFACE_COUNT; i++)
				{
					int index = i * VERETEX_COUNT_PER_SURFACE;

					for (int j = 0; j < VERETEX_COUNT_PER_SURFACE; j++)
					{
						int idx = _VertexOrder[index + j];
						outStream.Append(v[idx]);
					}

					outStream.RestartStrip();
				}
			}

			fixed4 frag(g2f i) : COLOR
			{
				return i.color;
			}

			ENDCG
		}
	}
}
