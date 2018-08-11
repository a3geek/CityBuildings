#ifndef GEOMETRIES_ROUNDED_INCLUDED
#define GEOMETRIES_ROUNDED_INCLUDED

#define ROUNDED_STEP radians(10.0)
#define ROUNDED_APPEND_LOOP_COUNT 2

#define ROUNDED_ANGLE_PER_LOOP (UNITY_TWO_PI / ROUNDED_APPEND_LOOP_COUNT)
#define ROUNDED_STEP_COUNT (ROUNDED_ANGLE_PER_LOOP / ROUNDED_STEP)

#define ROUNDED_VERTEX_COUNT_PER_LOOP 6
#define ROUNDED_APPEND_VERTEX_COUNT (ROUNDED_STEP_COUNT * ROUNDED_VERTEX_COUNT_PER_LOOP)

#define ROUNDED_APPEND_COUNT_PER_GEOMETRY (ROUNDED_APPEND_VERTEX_COUNT / ROUNDED_APPEND_LOOP_COUNT)
#define ROUNDED_ANGLE_OFFSET_PER_LOOP (UNITY_TWO_PI / ROUNDED_APPEND_LOOP_COUNT)

#define ROUNDED_SKIP_ANGLE radians(90.0)
#define ROUNDED_SKIP_LOOP_COUNT ((ROUNDED_SKIP_ANGLE / ROUNDED_STEP) - 1)

void AppendRounded(float3 center, float3 size, float3 uvStep, int loop, uint randSeed, inout TriangleStream<g2f> outStream)
{
	g2f lowCen, highCen;
	lowCen.uv = highCen.uv = 0.0;

	lowCen.pos = mul(UNITY_MATRIX_VP, float4(center.xyz, 1.0));
	highCen.pos = mul(UNITY_MATRIX_VP, float4(center.x, center.y + size.y, center.z, 1.0));

	float r = loop * ROUNDED_ANGLE_OFFSET_PER_LOOP;
	float max = r + ROUNDED_ANGLE_OFFSET_PER_LOOP;
	float step = ROUNDED_STEP;
	float3 yOffset = float3(0.0, size.y, 0.0);


	bool skiped = false;
	int count = floor(ROUNDED_STEP_COUNT);
	for (int i = 0; i < count; i++)
	{
		float r2 = r + step;

		float skip = 0.0;
		if(skiped == false && rand01(randSeed) < 0.5)
		{
			skiped = true;
			skip = ROUNDED_SKIP_ANGLE - step;
			r2 += skip;
			i += ROUNDED_SKIP_LOOP_COUNT;
		}

		r2 = min(r2, max);

		float3 p0 = center + float3(cos(r) * size.x * 0.5, size.y, sin(r) * size.z * 0.5);
		float3 p1 = center + float3(cos(r2) * size.x * 0.5, size.y, sin(r2) * size.z * 0.5);

		g2f v1, v2, v3, v4;

		v1.uv = v2.uv = v3.uv = v4.uv = 0.0;
		v1.pos = mul(UNITY_MATRIX_VP, float4(p0.xyz, 1.0));
		v2.pos = mul(UNITY_MATRIX_VP, float4(p1.xyz, 1.0));

		v3.pos = mul(UNITY_MATRIX_VP, float4(p0.xyz - yOffset, 1.0));
		v4.pos = mul(UNITY_MATRIX_VP, float4(p1.xyz - yOffset, 1.0));

		// high.
		outStream.Append(highCen);
		outStream.Append(v2);
		outStream.Append(v1);

		// low.
		outStream.Append(v4);
		outStream.Append(v3);
		outStream.Append(lowCen);

		outStream.RestartStrip();
		r += step + skip;
	}
}

#endif
