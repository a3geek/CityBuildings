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
				uint buildType;
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

			#include "./Libs/Random.cginc"
			#include "./Geometries/Cube.cginc"
			#include "./Geometries/Rounded.cginc"

			uniform StructuredBuffer<data> _data;
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

				data d = _data[inst];

				if (d.buildType == 0)
				{
					if (id > 0)
					{
						return;
					}

					//AppendCube(d.center, d.size, d.uvStep, d.randSeed, outStream);
				}
				else if (d.buildType == 1)
				{
					float angleOffset = ROUNDED_ANGLE_OFFSET_PER_LOOP * id;
					AppendRounded(d.center, d.size, d.uvStep, angleOffset, d.randSeed, outStream);
				}
			}

			fixed4 frag(g2f i) : COLOR
			{
				return tex2D(_windowTex, i.uv);
			}

			ENDCG
		}
	}
}
