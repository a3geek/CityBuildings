#ifndef CUBE_CONSTS_INCLUDED
#define CUBE_CONSTS_INCLUDED

#define VERTEX_COUNT_PER_TOPOLOGY 3 // triangle
#define TOPOLOGY_COUNT_PER_SURFACE 2

#define VERTEX_POSITION_COUNT 8
#define VERETEX_COUNT_PER_SURFACE 4
#define SURFACE_COUNT 6

#define APPEND_VERTEX_COUNT_PER_SURFACE (VERTEX_COUNT_PER_TOPOLOGY * TOPOLOGY_COUNT_PER_SURFACE) // 3 vertices per topology * 2 topology
#define APPEND_VERTEX_COUNT (APPEND_VERTEX_COUNT_PER_SURFACE * SURFACE_COUNT) // 6  append vertices per surface * 6 surface
#define VERETEX_ORDER_COUNT (VERETEX_COUNT_PER_SURFACE * SURFACE_COUNT) // 4 vertices per surface * 6 surface

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

static const int _VertexOrder[VERETEX_ORDER_COUNT] =
{
	0, 1, 2, 3, // left
	5, 0, 7, 2, // far
	4, 5, 6, 7, // right
	1, 4, 3, 6, // near
	0, 5, 1, 4, // top
	3, 6, 2, 7  // bottom
};

#endif
