Shader "Unlit/ARKitStreamReceiver"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    
    half4 decodeY(v2f_img i) : SV_Target
    {
        float2 uv = i.uv;
        uv.y *= 0.5;
        float y = tex2D(_MainTex, uv).r;
        return half4(y, 0.0, 0.0, 1.0);
    }
    
    half4 decodeCbCr(v2f_img i) : SV_Target
    {
        float2 uv = i.uv;
        uv.y *= 0.5;
        float2 cbcr = tex2D(_MainTex, uv).gb;
        return half4(cbcr.x, cbcr.y, 0.0, 1.0);
    }
    
    half4 decodeStencil(v2f_img i) : SV_Target
    {
        float2 uv = i.uv;
        uv.y *= 0.5;
        uv.y += 0.5;
        float stencil = tex2D(_MainTex, uv).r;
        return half4(stencil, 0.0, 0.0, 1.0);
    }
    
    half4 decodeDepth(v2f_img i) : SV_Target
    {
        float2 uv = i.uv;
        uv.y *= 0.5;
        uv.y += 0.5;
        float depth = tex2D(_MainTex, uv).g;
        // improve the precision of near point
        depth = pow(depth * 10.0, 2.0);
        return half4(depth, 0.0, 0.0, 1.0);
    }
    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment decodeY
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment decodeCbCr
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment decodeStencil
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment decodeDepth
            ENDCG
        }
    }
}
