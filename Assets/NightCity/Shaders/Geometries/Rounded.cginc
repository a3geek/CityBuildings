#ifndef GEOMETRIES_ROUNDED_INCLUDED
#define GEOMETRIES_ROUNDED_INCLUDED

#define ROUNDED_STEP 10.0
#define ROUNDED_STEP_COUNT (360 / ROUNDED_STEP)

#define ROUNDED_VERTEX_COUNT_PER_STRIP 6
#define ROUNDED_APPEND_VERTEX_COUNT (ROUNDED_STEP_COUNT * ROUNDED_VERTEX_COUNT_PER_STRIP)

#define ROUNDED_APPEND_LOOP_COUNT 2
#define ROUNDED_APPEND_COUNT_PER_GEOMETRY (ROUNDED_APPEND_VERTEX_COUNT / ROUNDED_APPEND_LOOP_COUNT)
#define ROUNDED_ANGLE_OFFSET_PER_LOOP (UNITY_TWO_PI / ROUNDED_APPEND_LOOP_COUNT)

void AppendRounded(float3 center, float3 size, float3 uvStep, float angleOffset, uint randSeed, inout TriangleStream<g2f> outStream)
{
	g2f v;
	v.uv = 0.0;

	g2f lowCen, highCen;
	lowCen.uv = highCen.uv = 0.0;

	lowCen.pos = mul(UNITY_MATRIX_VP, float4(center.xyz, 1.0));
	highCen.pos = mul(UNITY_MATRIX_VP, float4(center.x, center.y + size.y, center.z, 1.0));

	float step = radians(ROUNDED_STEP);
	float r = angleOffset;
	int count = trunc(ROUNDED_STEP_COUNT + 0.5);

	for (int i = 0; i < count; i++)
	{
		r += step;
		float r2 = r + step;

		if (r2 >= UNITY_TWO_PI)
		{
			float buf = r;
			r = UNITY_TWO_PI;
			r2 = buf;
		}

		float3 high1 = center + float3(cos(r) * size.x * 0.5, size.y, sin(r) * size.z * 0.5);
		float3 high2 = center + float3(cos(r2) * size.x * 0.5, size.y, sin(r2) * size.z * 0.5);

		float3 low1 = high1 - float3(0.0, size.y, 0.0);
		float3 low2 = high2 - float3(0.0, size.y, 0.0);

		// high.
		outStream.Append(highCen);

		v.pos = mul(UNITY_MATRIX_VP, float4(high2.xyz, 1.0));
		outStream.Append(v);

		v.pos = mul(UNITY_MATRIX_VP, float4(high1.xyz, 1.0));
		outStream.Append(v);

		// low.
		v.pos = mul(UNITY_MATRIX_VP, float4(low2.xyz, 1.0));
		outStream.Append(v);

		v.pos = mul(UNITY_MATRIX_VP, float4(low1.xyz, 1.0));
		outStream.Append(v);

		outStream.Append(lowCen);

		outStream.RestartStrip();
	}
}

#endif
