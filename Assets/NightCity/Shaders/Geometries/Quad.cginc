#ifndef GEOMETRIES_QUAD_INCLUDED
#define GEOMETRIES_QUAD_INCLUDED

void AppendQuad(float2 pos, float size, float height, float uv3, inout TriangleStream<g2f> outStream)
{
    float2 s = float2(20, 20);
    float2 hs = s * 0.5;

    float3 forward = float3(1, 0, 0);
    float3 up = float3(0, 1, 0);
    float3 p = float3(pos.x, height, pos.y);
    
    float3 c = cross(forward, up) * hs.x;
    up = up * hs.y;

    float3 v1 = p + float3(-1 * 10, 0, -1 * 10);
    v1 = p + (-up - c);

    float3 v2 = p + float3(-1 * 10, 0, 1 * 10);
    v2 = p + (up - c);

    float3 v3 = p + float3(1 * 10, 0, -1 * 10);
    v3 = p + (-up + c);

    float3 v4 = p + float3(1 * 10, 0, 1 * 10);
    v4 = p + (up + c);


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


    //g.pos = mul(UNITY_MATRIX_VP, float4(pos.x - hs, height, pos.y - hs, 1.0));
    //g.uv = float3(0.0, 0.0, uv3);
    //outStream.Append(g);

    //g.pos = mul(UNITY_MATRIX_VP, float4(pos.x - hs, height, pos.y + hs, 1.0));
    //g.uv = float3(0.0, 1.0, uv3);
    //outStream.Append(g);

    //g.pos = mul(UNITY_MATRIX_VP, float4(pos.x + hs, height, pos.y - hs, 1.0));
    //g.uv = float3(1.0, 0.0, uv3);
    //outStream.Append(g);

    //g.pos = mul(UNITY_MATRIX_VP, float4(pos.x + hs, height, pos.y + hs, 1.0));
    //g.uv = float3(1.0, 1.0, uv3);
    //outStream.Append(g);


}

#endif
