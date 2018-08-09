#ifndef GEOMETRIES_ROUNDED_INCLUDED
#define GEOMETRIES_ROUNDED_INCLUDED

#define STEP 10.0;
#define STEP_COUNT 360 / STEP

void AppendRounded(float3 center, float3 size, float3 uvStep, uint randSeed, inout TriangleStream<g2f> outStream)
{
	g2f v;
	v.uv = 0.0;

	g2f lowCen, highCen;
	lowCen.uv = highCen.uv = 0.0;

	lowCen.pos = mul(UNITY_MATRIX_VP, float4(center.xyz, 1.0));
	highCen.pos = mul(UNITY_MATRIX_VP, float4(center.x, center.y + size.y, center.z, 1.0));

	float step = 10.0;
	float r = 0.0;

	int count = STEP_COUNT;
	//for(int i = 0; i < count; i++)
	for (int i = 0; i < count; i++)
	{
		r += step;
		float rad1 = (r * UNITY_PI) / 180.0;
		float rad2 = ((r + step) * UNITY_PI) / 180.0;

		float3 high1 = center + float3(cos(rad1) * size.x * 0.5, size.y, sin(rad1) * size.z * 0.5);
		float3 high2 = center + float3(cos(rad2) * size.x * 0.5, size.y, sin(rad2) * size.z * 0.5);

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
