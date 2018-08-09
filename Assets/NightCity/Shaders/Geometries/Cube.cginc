﻿#ifndef GEOMETRIES_CUBE_INCLUDED
#define GEOMETRIES_CUBE_INCLUDED

#include "./Common.cginc"

#define VERTEX_COUNT_PER_TOPOLOGY 3 // triangle
#define TOPOLOGY_COUNT_PER_SURFACE 2

#define VERTEX_POSITION_COUNT 8
#define VERTEX_COUNT_PER_SURFACE 4
#define SURFACE_COUNT 6

#define APPEND_VERTEX_COUNT_PER_SURFACE (VERTEX_COUNT_PER_TOPOLOGY * TOPOLOGY_COUNT_PER_SURFACE) // 3 vertices per topology * 2 topology
#define APPEND_VERTEX_COUNT (APPEND_VERTEX_COUNT_PER_SURFACE * SURFACE_COUNT) // 6  append vertices per surface * 6 surface
#define VERETEX_ORDER_COUNT (VERTEX_COUNT_PER_SURFACE * SURFACE_COUNT) // 4 vertices per surface * 6 surface

static const int3 _VertexPos[VERTEX_POSITION_COUNT] =
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

static const float2 _UvParam[VERTEX_POSITION_COUNT] =
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

static const int _VertexOrder[SURFACE_COUNT][VERTEX_COUNT_PER_SURFACE] =
{
	{ 0, 1, 2, 3 }, // left
	{ 5, 0, 7, 2 }, // far
	{ 4, 5, 6, 7 }, // right
	{ 1, 4, 3, 6 }, // near
	{ 0, 5, 1, 4 }, // top
	{ 3, 6, 2, 7 }  // bottom
};

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