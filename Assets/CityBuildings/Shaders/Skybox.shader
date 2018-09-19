Shader "Hidden/Skybox"
{
	Properties
	{
        _TopColor("Top Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _HorizonColor("Horizon Color", Color) = (0.0, 0.0, 0.5, 1.0)
        _FloorColor("Floor Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _Horizon("Horizon", Float) = -0.05
        _HorizonOffset("Horizon Offset", Float) = 0.75
	}
    SubShader
    {
        Tags{ "RenderType" = "Background" "Queue" = "Background" "PreviewType" = "SkyBox" }

        Pass
        {
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float3 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            float4 _TopColor, _HorizonColor, _FloorColor;
            float _Horizon, _HorizonOffset;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float d = dot(float3(0.0, 1.0, 0.0), i.texcoord);
                return d < _Horizon ? _FloorColor : lerp(_HorizonColor, _TopColor, saturate(d) + _HorizonOffset);
            }
            ENDCG
        }
    }
}
