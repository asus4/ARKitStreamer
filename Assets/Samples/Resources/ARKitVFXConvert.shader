Shader "Hidden/ARKitVFXConvert"
{
    Properties
    {
        _textureY ("Texture Y", 2D) = "white" {}
        _textureCbCr ("Texture CbCr", 2D) = "black" {}
        _textureStencil ("Texture CbCr", 2D) = "black" {}
        _texutreDepth ("Texture Depth", 2D) = "black" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _textureY;
    sampler2D _textureCbCr;
     sampler2D _textureStencil;
    sampler2D _texutreDepth;

    fixed4 colorConvert(v2f_img i) : SV_Target
    {
        // sample the texture
        float2 uv = i.uv;
        float y = tex2D(_textureY, uv).r;
        float4 ycbcr = float4(y, tex2D(_textureCbCr, uv).rg, 1.0);

        const float4x4 ycbcrToRGBTransform = float4x4(
            float4(1.0, +0.0000, +1.4020, -0.7010),
            float4(1.0, -0.3441, -0.7141, +0.5291),
            float4(1.0, +1.7720, +0.0000, -0.8860),
            float4(0.0, +0.0000, +0.0000, +1.0000)
        );

        float4 result = mul(ycbcrToRGBTransform, ycbcr);

#if !UNITY_COLORSPACE_GAMMA
        result = float4(GammaToLinearSpace(result.xyz), result.w);
#endif // !UNITY_COLORSPACE_GAMMA

        return result;
    }

    float4 positionConvert(v2f_img i) : SV_TARGET
    {
        float x = (i.uv.x - 0.5) * 2.0;
        float y = (i.uv.y - 0.5) * 2.0;
        float z = tex2D(_texutreDepth, i.uv).r;

        return float4(x, y, z, 1.0);
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment colorConvert
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment positionConvert
            ENDCG
        }
    }
}
