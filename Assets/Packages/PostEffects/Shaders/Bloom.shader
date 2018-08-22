Shader "Hidden/Bloom"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
        CGINCLUDE
        #pragma vertex vert_img
        #pragma fragment frag

        #include "UnityCG.cginc"

        sampler2D _MainTex, _SourceTex;
        float4 _MainTex_TexelSize;
        uniform float _Delta, _Intensity;
        uniform float4 _Filters;

        #include "./Libs/BoxFilteringBlur.cginc"
        
        float GetBrightness(float3 c)
        {
            return max(c.r, max(c.g, c.b));
        }

        ENDCG

        Cull Off
        ZTest Always
        ZWrite Off
        
        // 0: Extract pixels that to be applied based on Brightness.
        Pass
        {
            CGPROGRAM
            
            float4 frag(v2f_img i) : SV_Target
            {
                float3 col = BoxFilteringBlur(i.uv, 1.0);
                float brightness = GetBrightness(col.rgb);

                float soft = clamp(brightness - _Filters.y, 0.0, _Filters.z);
                soft = soft * soft * _Filters.w;
                
                float contri = max(soft, brightness - _Filters.x) / max(brightness, 0.00001);
                return float4(col * contri, 1.0);
            }

            ENDCG
        }

        // 1: Downsampling and Upsampling.
        Pass
        {
            CGPROGRAM

            float4 frag(v2f_img i) : SV_Target
            {
                return float4(BoxFilteringBlur(i.uv, _Delta).rgb, 1.0);
            }

            ENDCG
        }

        // 2: Last Upsampling.
        // Composition to source texture and blur texture.
        Pass
        {
            CGPROGRAM

            float4 frag(v2f_img i) : SV_Target
            {
                float4 col = tex2D(_SourceTex, i.uv);
                col.rgb += BoxFilteringBlur(i.uv, 0.5) * _Intensity;
                return col;
            }

            ENDCG
        }
	}
}
