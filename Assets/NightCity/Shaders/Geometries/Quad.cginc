#ifndef GEOMETRIES_QUAD_INCLUDED
#define GEOMETRIES_QUAD_INCLUDED

void AppendQuad(float2 pos, float size, float height, inout TriangleStream<g2f> outStream)
{
    g2f g;
    float hs = 0.5 * size;

    g.pos = mul(UNITY_MATRIX_VP, float4(pos.x - hs, height, pos.y - hs, 1.0));
    g.uv = float2(0.0, 0.0);
    outStream.Append(g);

    g.pos = mul(UNITY_MATRIX_VP, float4(pos.x - hs, height, pos.y + hs, 1.0));
    g.uv = float2(0.0, 1.0);
    outStream.Append(g);

    g.pos = mul(UNITY_MATRIX_VP, float4(pos.x + hs, height, pos.y - hs, 1.0));
    g.uv = float2(1.0, 0.0);
    outStream.Append(g);

    g.pos = mul(UNITY_MATRIX_VP, float4(pos.x + hs, height, pos.y + hs, 1.0));
    g.uv = float2(1.0, 1.0);
    outStream.Append(g);

    outStream.RestartStrip();
}

#endif
