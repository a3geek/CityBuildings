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
			#include "./Libs/Random.cginc"
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
				float3 uvStep;
				uint randSeed;
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

			v2g vert(uint id : SV_VertexID, uint inst : SV_InstanceID)
			{
				v2g o;
				o.index = inst;

				return o;
			}

			float GetOffset(uint randSeed, float base, float max)
			{
				float width = max - base;
				float div = width / 8.0;

				return trunc(rand(randSeed, div)) / max;
			}

			void AppendCube(float3 center, float3 size, float3 uvStep, uint randSeed, float yOffset, inout TriangleStream<g2f> outStream)
			{
				g2f v;

				float wc = 1024.0 / 8.0;
				float hc = 1024.0 / 8.0;
				float dc = 1024.0 / 8.0;

				float randWD = uvStep.x > uvStep.z ?
					GetOffset(randSeed, uvStep.x, wc) :
					GetOffset(randSeed, uvStep.z, dc);
				float randH = GetOffset(randSeed, uvStep.y, hc);

				for (int i = 0; i < SURFACE_COUNT; i++)
				{
					for (int j = 0; j < VERTEX_COUNT_PER_SURFACE; j++)
					{
						int idx = _VertexOrder[i][j];

						float3 pos = center + _VertexPos[idx] * size * 0.5 + float3(0.0, yOffset, 0.0);
						v.pos = mul(UNITY_MATRIX_VP, float4(pos, 1.0));

						v.uv = float2(randWD, randH) + _UvParam[idx] * float2(
							i % 2.0 == 0.0 ? uvStep.z / dc : uvStep.x / wc,
							uvStep.y / hc
						);

						outStream.Append(v);
					}

					outStream.RestartStrip();
				}
			}

			[maxvertexcount(APPEND_VERTEX_COUNT * 2)]
			void geom(point v2g input[1], inout TriangleStream<g2f> outStream)
			{
				data d = _data[input[0].index];

				AppendCube(d.center, d.baseSize, 0.0, 0, d.baseSize.y * 0.5, outStream);
				AppendCube(d.center, d.size, d.uvStep, d.randSeed, d.baseSize.y * 0.5 + d.size.y * 0.5, outStream);
			}

			fixed4 frag(g2f i) : COLOR
			{
				return tex2D(_windowTex, i.uv);
			}

			ENDCG
		}
	}
}
