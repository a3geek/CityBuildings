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

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma target 5.0

			struct data
			{
				float3 center;
				float3 size;
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

			#include "./Libs/Random.cginc"
			#include "./Libs/Appenders.cginc"

			#define DOUBLE_VERTEX_POSITION_COUNT VERTEX_POSITION_COUNT * 2

			uniform StructuredBuffer<data> _data;
			uniform sampler2D _windowTex;

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

				AppendCube(d.center, d.size, d.uvStep, d.randSeed, outStream);
			}

			fixed4 frag(g2f i) : COLOR
			{
				return tex2D(_windowTex, i.uv);
			}

			ENDCG
		}
	}
}
