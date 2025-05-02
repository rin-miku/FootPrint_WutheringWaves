Shader "Custom/FootPrint"
{
    Properties
    {
        _FootprintNormal ("Normal", 2D) = "bump" {}
        _FootprintMask ("Mask", 2D) = "white" {}
        _FootUV ("UV Position", Vector) = (0, 0, 0, 0)
        _FootprintScale ("Footprint Scale", float) = 0.01 
    }
    SubShader
    {
        Pass
        {
            Tags { "RenderType"="Opaque" }
            LOD 200

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            #define MAX_FOOTPRINTS 100

            sampler2D _FootprintNormal;
            sampler2D _FootprintMask;
            int _FootprintCount;
            float4 _FootUVArray[MAX_FOOTPRINTS];
            float _FootprintScale;

            float4 frag(v2f_customrendertexture i) : SV_Target
            {
                float4 encodedNormal = float4(0, 0, 0, 0);

                [unroll]
                for(int idx = 0; idx < _FootprintCount; idx++)
                {
                    float2 center = _FootUVArray[idx].xy;

                    float2 delta = i.localTexcoord.xy - center;
                    float angle = radians(_FootUVArray[idx].w);
                    float2x2 rotMatrix = float2x2(
                        cos(angle), -sin(angle),
                        sin(angle), cos(angle));
                    delta = mul(rotMatrix, delta);

                    if (abs(delta.x) > _FootprintScale || abs(delta.y) > _FootprintScale)
                        continue;

                    float2 localUV = delta / _FootprintScale / 2 + 0.5;
                    if(tex2D(_FootprintMask, localUV).r < 0.5)
                        continue;

                    float3 decalNormal = UnpackNormal(tex2D(_FootprintNormal, localUV));
                    float fade = _FootUVArray[idx].z;

                    encodedNormal.rgb = decalNormal * 0.5 + 0.5;
                    encodedNormal.a = fade;
                }

                return encodedNormal;
            }
            ENDCG
        }
    }
}
