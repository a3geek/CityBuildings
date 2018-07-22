Shader "Unlit/Skyscraper"
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
			#include "./Consts/Cube.cginc"

			#define DOUBLE_VERTEX_POSITION_COUNT VERTEX_POSITION_COUNT * 2

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma target 5.0

			struct data
			{
				float3 center;
				float4 color;
				float3 size;
				float3 baseSize;
			};

			struct v2g
			{
				uint index : ANY;
			};

			struct g2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};

			uniform StructuredBuffer<data> _data;

			void AppendCube(in g2f v[DOUBLE_VERTEX_POSITION_COUNT], uniform int offset, inout TriangleStream<g2f> outStream)
			{
				for (int i = 0; i < SURFACE_COUNT; i++)
				{
					int index = i * VERETEX_COUNT_PER_SURFACE;

					for (int j = 0; j < VERETEX_COUNT_PER_SURFACE; j++)
					{
						int idx = _VertexOrder[index + j];
						outStream.Append(v[idx + offset]);
					}

					outStream.RestartStrip();
				}
			}

			v2g vert(uint id : SV_VertexID, uint inst : SV_InstanceID)
			{
				v2g o;
				o.index = inst;

				return o;
			}

			[maxvertexcount(APPEND_VERTEX_COUNT * 2)]
			void geom(point v2g input[1], inout TriangleStream<g2f> outStream)
			{
				data d = _data[input[0].index];

				g2f v[DOUBLE_VERTEX_POSITION_COUNT];

				for (int i = 0; i < VERTEX_POSITION_COUNT; i++)
				{
					float3 pos = d.center + _VertexPos[i] * d.size * 0.5f;
					float3 basePos = d.center + _VertexPos[i] * d.baseSize * 0.5f;

					v[i].color = d.color;
					v[i].pos = mul(UNITY_MATRIX_VP, float4(pos, 1.0f));

					v[VERTEX_POSITION_COUNT + i].color = d.color;
					v[VERTEX_POSITION_COUNT + i].pos = mul(UNITY_MATRIX_VP, float4(basePos, 1.0f));
				}

				AppendCube(v, 0, outStream);
				AppendCube(v, VERTEX_POSITION_COUNT, outStream);
			}

			fixed4 frag(g2f i) : COLOR
			{
				return i.color;
			}

			ENDCG
		}
	}
}
