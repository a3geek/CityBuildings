#ifndef GEOMETRIES_QUAD_INCLUDED
#define GEOMETRIES_QUAD_INCLUDED

void AppendQuad(float3 pos, float2 size, float3 forward, float3 up, float uv3, inout TriangleStream<g2f> outStream)
{
    float2 hs = size * 0.5;

    float3 c = cross(forward, up) * hs.x;
    float3 u = up * hs.y;

    float3 v1 = pos + (-u - c);
    float3 v2 = pos + (u - c);
    float3 v3 = pos + (-u + c);
    float3 v4 = pos + (u + c);

    g2f g;

    g.pos = mul(UNITY_MATRIX_VP, float4(v1.xyz, 1.0));
    g.uv = float3(0.0, 0.0, uv3);
    outStream.Append(g);

    g.pos = mul(UNITY_MATRIX_VP, float4(v2.xyz, 1.0));
    g.uv = float3(0.0, 1.0, uv3);
    outStream.Append(g);

    g.pos = mul(UNITY_MATRIX_VP, float4(v3.xyz, 1.0));
    g.uv = float3(1.0, 0.0, uv3);
    outStream.Append(g);

    g.pos = mul(UNITY_MATRIX_VP, float4(v4.xyz, 1.0));
    g.uv = float3(1.0, 1.0, uv3);
    outStream.Append(g);

    outStream.RestartStrip();
}

#endif
