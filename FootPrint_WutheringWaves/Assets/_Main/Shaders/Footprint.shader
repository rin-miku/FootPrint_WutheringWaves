Shader "Custom/Footprint"
{
    Properties
    {
        _FootprintNormal ("Normal", 2D) = "bump" {}
        _FootprintMask ("Mask", 2D) = "white" {}
        _FootUV ("UV Position", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define MAX_FOOTPRINTS 100

            sampler2D _FootprintNormal;
            sampler2D _FootprintMask;
            int _FootprintCount;
            float4 _FootUVArray[MAX_FOOTPRINTS];
            float _FadeArray[MAX_FOOTPRINTS];

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 encodedNormal = float4(0, 0, 0, 0);

                for(int idx = 0; idx < _FootprintCount; idx++)
                {
                    float2 center = _FootUVArray[idx].xy;

                    float2 delta = i.uv - center;
                    if (abs(delta.x) > 0.0625 || abs(delta.y) > 0.0625)
                        continue;

                    float2 localUV = delta / 0.125 + 0.5;
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
