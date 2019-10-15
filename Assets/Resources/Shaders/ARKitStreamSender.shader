Shader "Unlit/ARKitStreamSender"
{
    Properties
    {
        _textureY ("TextureY", 2D) = "white" {}
        _textureCbCr ("TextureCbCr", 2D) = "black" {}
        _textureStencil ("Texture Stencil", 2D) = "black" {}
        _textureDepth ("Texture Depth", 2D) = "black" {}
    }
    SubShader
    {
        Cull Off
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 position : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                o.texcoord = v.texcoord;
                return o;
            }

            // samplers
            sampler2D _textureY;
            sampler2D _textureCbCr;
            sampler2D _textureStencil;
            sampler2D _textureDepth;

            half4 frag (v2f i) : SV_Target
            {
                float2 texcoord = i.texcoord;

                // 0.0 < uv.y < 0.5
                // R: Y
                // G: Cb
                // B: Cr
                if(i.texcoord.y < 0.5)
                {
                    // sample the texture
                    texcoord.y *= 2.0;

                    float y = tex2D(_textureY, texcoord).r;
                    float2 cbcr = tex2D(_textureCbCr, texcoord).rg;
                    float4 ycbcr = float4(y, cbcr, 1.0);

                    return float4(y, cbcr.x, cbcr.y, 1.0);

                    // const float4x4 ycbcrToRGBTransform = float4x4(
                    //     float4(1.0, +0.0000, +1.4020, -0.7010),
                    //     float4(1.0, -0.3441, -0.7141, +0.5291),
                    //     float4(1.0, +1.7720, +0.0000, -0.8860),
                    //     float4(0.0, +0.0000, +0.0000, +1.0000)
                    // );

                    // return mul(ycbcrToRGBTransform, ycbcr);
                }
                // else
                // 0.5 < uv.y < 1.0
                // R: Stencil
                // G: Depth
                // B: Not Used
                texcoord.y = (texcoord.y - 0.5) * 2.0;
               
                float stencil = tex2D(_textureStencil, texcoord).r;
                float depth = tex2D(_textureDepth, texcoord).r;

                return float4(stencil, depth, 0.0, 1.0);
            }
            ENDCG
        }
    }
}

