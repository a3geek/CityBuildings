#ifndef GEOMETRIES_SPHERE_INCLUDED
#define GEOMETRIES_SPHERE_INCLUDED

#define SPHERE_THETA_COUNT 5
#define SPHERE_PHI_COUNT 10

#define SPHERE_STEP_THETA (UNITY_PI / (SPHERE_THETA_COUNT + 1))
#define SPHERE_STEP_PHI (UNITY_TWO_PI / SPHERE_PHI_COUNT)

#define SPHERE_VERTEX_COUNT (SPHERE_THETA_COUNT * SPHERE_PHI_COUNT + 2)
#define SPHERE_APPEND_VERTEX_COUNT (2 * SPHERE_THETA_COUNT * SPHERE_PHI_COUNT + 2 * SPHERE_PHI_COUNT)

float3 getCoordinates(float theta, float phi)
{
    return float3(
        sin(theta) * cos(phi),
        cos(theta),
        sin(theta) * sin(phi)
    );
}

float4 getPos(float3 center, float radius, float theta, float phi)
{
    return mul(UNITY_MATRIX_VP, float4(center + radius * getCoordinates(theta, phi), 1.0));
}

void AppendSphere(float3 pos, float radius, float uv3, inout TriangleStream<g2f> outStream)
{
    g2f output[SPHERE_VERTEX_COUNT];
    float4 uv = float4(0.0, 0.0, uv3, 0.0);

    int id = 0;
    output[id].pos = getPos(pos, radius, 0.0, 0.0);
    output[id++].uv = uv;

    [unroll]
    for (int i = 1; i <= SPHERE_THETA_COUNT; i++)
    {
        float theta = SPHERE_STEP_THETA * i;

        [unroll]
        for (int j = 0; j < SPHERE_PHI_COUNT; j++)
        {
            float phi = SPHERE_STEP_PHI * j;
            output[id].pos = getPos(pos, radius, theta, phi);
            output[id++].uv = float4(phi / UNITY_TWO_PI, theta / UNITY_TWO_PI, uv3, 0.0);
        }
    }

    output[id].pos = getPos(pos, radius, UNITY_PI, 0.0);
    output[id++].uv = float4(0.0, UNITY_PI / UNITY_TWO_PI, uv3, 0.0);

    for (i = 1; i <= SPHERE_PHI_COUNT; i++)
    {
        outStream.Append(output[0]);

        for (int j = 0; j < SPHERE_THETA_COUNT; j++)
        {
            int index = j * SPHERE_PHI_COUNT + i;
            int next = index + 1;

            next = next - SPHERE_PHI_COUNT * min(max(next - SPHERE_PHI_COUNT * (j + 1), 0), 1);

            outStream.Append(output[next]);
            outStream.Append(output[index]);
        }

        outStream.Append(output[SPHERE_VERTEX_COUNT - 1]);
        outStream.RestartStrip();
    }
}

#endif
