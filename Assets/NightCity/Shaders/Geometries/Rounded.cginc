#ifndef GEOMETRIES_ROUNDED_INCLUDED
#define GEOMETRIES_ROUNDED_INCLUDED

#include "./Common.cginc"

#define ROUNDED_STEP radians(10.0)
#define ROUNDED_APPEND_LOOP_COUNT 3

#define ROUNDED_ANGLE_PER_LOOP (UNITY_TWO_PI / ROUNDED_APPEND_LOOP_COUNT)
#define ROUNDED_STEP_COUNT floor(ROUNDED_ANGLE_PER_LOOP / ROUNDED_STEP)

#define ROUNDED_VERTEX_COUNT_PER_LOOP 10
#define ROUNDED_APPEND_VERTEX_COUNT (ROUNDED_STEP_COUNT * ROUNDED_VERTEX_COUNT_PER_LOOP)

#define ROUNDED_APPEND_COUNT_PER_GEOMETRY (ROUNDED_APPEND_VERTEX_COUNT / ROUNDED_APPEND_LOOP_COUNT)
#define ROUNDED_ANGLE_OFFSET_PER_LOOP (UNITY_TWO_PI / ROUNDED_APPEND_LOOP_COUNT)

#define ROUNDED_SKIP_ANGLE(i) (i == 0 ? radians(45.0) : radians(90.0))
#define ROUNDED_SKIP_LOOP_COUNT(angle) ((angle / ROUNDED_STEP) - 1)

void AppendRounded(float3 center, float3 size, float3 uvRange, int loop, uint2 seed, int index, inout TriangleStream<g2f> outStream)
{
	g2f lowCen, highCen;
	lowCen.uv = highCen.uv = float4(0.0, 0.0, -1.0, UnityObjectToViewPos(float4(center, 1.0)).z);

	lowCen.pos = mul(UNITY_MATRIX_VP, float4(center.xyz, 1.0));
	highCen.pos = mul(UNITY_MATRIX_VP, float4(center.x, center.y + size.y, center.z, 1.0));

	float r = loop * ROUNDED_ANGLE_OFFSET_PER_LOOP;
	float max = r + ROUNDED_ANGLE_OFFSET_PER_LOOP;
	float step = ROUNDED_STEP;

	bool skiped = false;
	int count = ROUNDED_STEP_COUNT;

	float uvWidth = (uvRange.x + uvRange.z) * 1.5;
	float uvPerLoop = uvWidth / UNITY_TWO_PI;
	float2 uvOffset = float2(GetUvOffset(seed.y, uvWidth, _WindowNumberX), GetUvOffset(seed.y, uvRange.y, _WindowNumberY));

	float3 yOffset = float3(0.0, size.y, 0.0);
	float3 halfSize = 0.5 * size;
	float uvY = uvRange.y / _WindowNumberY;

	for (int i = 0; i < count; i++)
	{
		float r2 = r + step;

		float skip = 0.0;
		if(skiped == false && rand01(seed.x) < 0.1)
		{
			skiped = true;
			skip = ROUNDED_SKIP_ANGLE(round(rand01(seed.x)));
			i += ROUNDED_SKIP_LOOP_COUNT(skip);
		}
		r2 = min(r2 + skip, max);

		float3 p0 = center + float3(cos(r) * halfSize.x, size.y, sin(r) * halfSize.z);
		float3 p1 = center + float3(cos(r2) * halfSize.x, size.y, sin(r2) * halfSize.z);

		float uvX0 = (r * uvPerLoop) / _WindowNumberX;
		float uvX1 = (r2 * uvPerLoop) / _WindowNumberX;

		g2f v1, v2, v3, v4;

		v1.pos = mul(UNITY_MATRIX_VP, float4(p0.xyz, 1.0));
		v2.pos = mul(UNITY_MATRIX_VP, float4(p1.xyz, 1.0));

		v3.pos = mul(UNITY_MATRIX_VP, float4(p0.xyz - yOffset, 1.0));
		v4.pos = mul(UNITY_MATRIX_VP, float4(p1.xyz - yOffset, 1.0));

		// high.
		v1.uv = v2.uv = float4(0.0, 0.0, -1.0, UnityObjectToViewPos(float4(center, 1.0)).z);
		outStream.Append(highCen);
		outStream.Append(v2);
		outStream.Append(v1);

		// side.
		v1.uv = float4(uvOffset + float2(uvX0, uvY), index, UnityObjectToViewPos(float4(p0.xyz, 1.0)).z);
		v2.uv = float4(uvOffset + float2(uvX1, uvY), index, UnityObjectToViewPos(float4(p1.xyz, 1.0)).z);
		v3.uv = float4(uvOffset + float2(uvX0, 0.0), index, UnityObjectToViewPos(float4(p0.xyz - yOffset, 1.0)).z);
		v4.uv = float4(uvOffset + float2(uvX1, 0.0), index, UnityObjectToViewPos(float4(p1.xyz - yOffset, 1.0)).z);

		outStream.Append(v2);
		outStream.Append(v1);
		outStream.Append(v4);
		outStream.Append(v3);

		// low.
		v3.uv = v4.uv = float4(0.0, 0.0, -1.0, UnityObjectToViewPos(float4(center, 1.0)).z);
		outStream.Append(v4);
		outStream.Append(v3);
		outStream.Append(lowCen);

		outStream.RestartStrip();
		r += (step + skip);
	}
}

#endif
