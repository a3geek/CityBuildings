#ifndef IncludedAppenders
#define IncludedAppenders
#include "../Consts/Cube.cginc"

float GetOffset(uint randSeed, float base, float max)
{
	float width = max - base;
	float div = width / 8.0;

	return trunc(rand(randSeed, div)) / max;
}

void AppendCube(float3 center, float3 size, float3 uvStep, uint randSeed, inout TriangleStream<g2f> outStream)
{
	g2f v;

	float wc = 1024.0 / 8.0;
	float hc = 1024.0 / 8.0;
	float dc = 1024.0 / 8.0;

	float randWD = uvStep.x > uvStep.z ?
		GetOffset(randSeed, uvStep.x, wc) :
		GetOffset(randSeed, uvStep.z, dc);
	float randH = GetOffset(randSeed, uvStep.y, hc);

	float3 halfSize = 0.5 * size;

	for (int i = 0; i < SURFACE_COUNT; i++)
	{
		for (int j = 0; j < VERTEX_COUNT_PER_SURFACE; j++)
		{
			int idx = _VertexOrder[i][j];

			float3 pos = center + _VertexPos[idx] * halfSize;
			pos.y += halfSize.y;
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

#endif
