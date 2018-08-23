#ifndef GEOMETRIES_CUBE_INCLUDED
#define GEOMETRIES_CUBE_INCLUDED

#include "./Common.cginc"

#define CUBE_VERTEX_COUNT_PER_TOPOLOGY 3 // triangle
#define CUBE_TOPOLOGY_COUNT_PER_SURFACE 2

#define CUBE_VERTEX_POSITION_COUNT 8
#define CUBE_VERTEX_COUNT_PER_SURFACE 4
#define CUBE_SURFACE_COUNT 6

#define CUBE_APPEND_VERTEX_COUNT_PER_SURFACE (CUBE_VERTEX_COUNT_PER_TOPOLOGY * CUBE_TOPOLOGY_COUNT_PER_SURFACE) // 3 vertices per topology * 2 topology
#define CUBE_APPEND_VERTEX_COUNT (CUBE_APPEND_VERTEX_COUNT_PER_SURFACE * CUBE_SURFACE_COUNT) // 6  append vertices per surface * 6 surface
#define CUBE_VERETEX_ORDER_COUNT (CUBE_VERTEX_COUNT_PER_SURFACE * CUBE_SURFACE_COUNT) // 4 vertices per surface * 6 surface

static const int3 _CubeVertexPos[CUBE_VERTEX_POSITION_COUNT] =
{
	int3(-1, +1, +1),  // left_top_far ... 0
	int3(-1, +1, -1),  // left_top_near ... 1
	int3(-1, -1, +1),  // left_bottom_far ... 2
	int3(-1, -1, -1),  // left_bottom_near ... 3
	int3(+1, +1, -1),  // right_top_near ... 4
	int3(+1, +1, +1),  // right_top_far ... 5
	int3(+1, -1, -1),  // right_bottom_near ... 6
	int3(+1, -1, +1),  // right_bottom_far ... 7
};

static const float2 _CubeUvParam[CUBE_VERTEX_POSITION_COUNT] =
{
	float2(1.0, 1.0),
	float2(0.0, 1.0),
	float2(1.0, 0.0),
	float2(0.0, 0.0),
	float2(1.0, 1.0),
	float2(0.0, 1.0),
	float2(1.0, 0.0),
	float2(0.0, 0.0),
};

static const int _CubeVertexOrder[CUBE_SURFACE_COUNT][CUBE_VERTEX_COUNT_PER_SURFACE] =
{
	{ 0, 1, 2, 3 }, // left
	{ 5, 0, 7, 2 }, // far
	{ 4, 5, 6, 7 }, // right
	{ 1, 4, 3, 6 }, // near
	{ 0, 5, 1, 4 }, // top
	{ 3, 6, 2, 7 }  // bottom
};

void AppendCube(float3 center, float3 size, float3 uvRange, uint randSeed, int index, inout TriangleStream<g2f> outStream)
{
	g2f v;

	float wc = 1024.0 / 8.0;
	float hc = 1024.0 / 8.0;
	float dc = 1024.0 / 8.0;

	float randWD = uvRange.x > uvRange.z ?
		GetUvOffset(randSeed, uvRange.x, wc) :
		GetUvOffset(randSeed, uvRange.z, dc);
	float randH = GetUvOffset(randSeed, uvRange.y, hc);
	float2 uvOffset = float2(randWD, randH);

	float3 halfSize = 0.5 * size;

	for (int i = 0; i < CUBE_SURFACE_COUNT; i++)
	{
		for (int j = 0; j < CUBE_VERTEX_COUNT_PER_SURFACE; j++)
		{
			int idx = _CubeVertexOrder[i][j];

			float3 pos = center + _CubeVertexPos[idx] * halfSize;
			pos.y += halfSize.y;
			v.pos = mul(UNITY_MATRIX_VP, float4(pos, 1.0));

			v.uv = float3(uvOffset + _CubeUvParam[idx] * float2(
				IsMultipleOfTwo(i) == true ? uvRange.z / dc : uvRange.x / wc,
				uvRange.y / hc
			), index);

			outStream.Append(v);
		}

		outStream.RestartStrip();
	}
}

#endif
