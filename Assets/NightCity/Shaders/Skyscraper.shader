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
				float2 uv : TEXCOORD0;
			};

			uniform StructuredBuffer<data> _data;
			uniform sampler2D _windowTex;

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
					float3 basePos = d.center + _VertexPos[i] * d.baseSize * 0.5f;
					float3 pos = d.center + _VertexPos[i] * d.size * 0.5f;

					basePos.y += d.baseSize.y * 0.5f;
					pos.y += d.baseSize.y * 0.5f + d.size.y * 0.5f;

					float2 uv = _UvParam[i] * (64.0 / 1024.0);

					v[i].pos = mul(UNITY_MATRIX_VP, float4(pos, 1.0f));
					v[i].uv = uv;

					v[VERTEX_POSITION_COUNT + i].pos = mul(UNITY_MATRIX_VP, float4(basePos, 1.0f));
					v[VERTEX_POSITION_COUNT + i].uv = 0.0;
				}

				AppendCube(v, 0, outStream);
				AppendCube(v, VERTEX_POSITION_COUNT, outStream);
			}

			fixed4 frag(g2f i) : COLOR
			{
				return tex2D(_windowTex, i.uv);
			}

			ENDCG
		}
	}
}
