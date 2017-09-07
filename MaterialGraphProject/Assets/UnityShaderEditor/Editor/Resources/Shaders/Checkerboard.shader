Shader "Hidden/Checkerboard"
{
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform float _X;
            uniform float _Y;

            static const float4 col1 = float4(0.15, 0.15, 0.15, 1.0);
            static const float4 col2 = float4(0.3, 0.3, 0.3, 1.0);

            float4 frag(v2f_img i) : COLOR
            {
                float total = floor(i.uv.x * _X) + floor(i.uv.y * _Y);
                bool isEven = total % 2.0 == 0.0;
                return isEven ? col1 : col2;
            }
            ENDCG
        }
    }
}
