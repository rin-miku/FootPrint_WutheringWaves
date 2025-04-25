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

            sampler2D _FootprintNormal;
            sampler2D _FootprintMask;
            float4 _FootUV;

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
                float2 center = _FootUV.xy;

                float2 delta = i.uv - center;
                if (abs(delta.x) > 0.0625 || abs(delta.y) > 0.0625)
                    discard;

                float2 localUV = delta / 0.125 + 0.5;
                if(tex2D(_FootprintMask, localUV).r < 0.5)
                    discard;

                float3 decalNormal = UnpackNormal(tex2D(_FootprintNormal, localUV));
                float4 encodedNormal;
                encodedNormal.rgb = decalNormal * 0.5 + 0.5;
                encodedNormal.a = 1;

                return encodedNormal;
            }
            ENDCG
        }
    }
}
