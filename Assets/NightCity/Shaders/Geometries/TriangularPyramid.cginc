#ifndef GEOMETRIES_TRIANGULAR_PYRAMID_INCLUDED
#define GEOMETRIES_TRIANGULAR_PYRAMID_INCLUDED

static const int2 _TriangularPyramidEveryDirection[4] = {
    int2(1, 1), int2(-1, 1), int2(-1, -1), int2(1, -1)
};

void AppendTriangularPyramid(float3 pos, float3 size, inout TriangleStream<g2f> outStream)
{
    g2f g;
    g.uv = 0.0;

    float4 top = mul(UNITY_MATRIX_VP, float4(pos + float3(0.0, size.y, 0.0), 1.0));
    float2 center = float2(pos.x, pos.z);

    [unroll]
    for (int i = 0; i < 4; i++) {
        int2 dir1 = _TriangularPyramidEveryDirection[i];
        int2 dir2 = _TriangularPyramidEveryDirection[i + 1 >= 4 ? i - 3 : i + 1];

        float2 p1 = center + dir1 * size.xz;
        float2 p2 = center + dir2 * size.xz;

        g.pos = mul(UNITY_MATRIX_VP, float4(p1.x, pos.y, p1.y, 1.0));
        outStream.Append(g);

        g.pos = top;
        outStream.Append(g);

        g.pos = mul(UNITY_MATRIX_VP, float4(p2.x, pos.y, p2.y, 1.0));
        outStream.Append(g);

        outStream.RestartStrip();
    }
}

#endif
