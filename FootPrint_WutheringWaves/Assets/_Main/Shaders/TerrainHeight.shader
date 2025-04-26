Shader "Custom/TerrainHeight"
{
    Properties
    {
        _TrialTexture ("Trial Texture", 2D) = "white" {}
        _TrailOffset ("Trial Offset", float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Lighting Off

        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            sampler2D _TrialTexture;
            float _TrailOffset;
            float4 _TrialPosition;
            float _TrailAngle;

            float4 frag(v2f_customrendertexture i) : SV_target
            {
                float4 oldHeight = tex2D(_SelfTexture2D, i.localTexcoord.xy);
                
                float2 pos = i.localTexcoord.xy - _TrialPosition;
                float2x2 rotMatrix = float2x2(
                    cos(_TrailAngle), -sin(_TrailAngle),
                    sin(_TrailAngle), cos(_TrailAngle));
                pos = mul(rotMatrix, pos);
                pos = pos / _TrailOffset + float2(0.5, 0.5);
                
                float4 newHeight = tex2D(_TrialTexture, pos);

                return min(oldHeight, newHeight);
            }
            ENDCG
        }
    }
}
