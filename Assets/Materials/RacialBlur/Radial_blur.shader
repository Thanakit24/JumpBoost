Shader "Custom/RadialBlur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        numSamples("Number of samples", int) = 10
        blurWidth("Width of the blur", float) = 0.1
        center("Normalized center", Vector) = (0.5, 0.5, 0, 0)
    }
    SubShader
    {

//        Blend One One
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties
            sampler2D _MainTex;
            int numSamples;
            float blurWidth;
            float2 center;
            
            //https://www.shadertoy.com/view/XsfSDs
            //Radial Blur
            float4 frag(v2f_img input) : COLOR
            {
                float blurStart = 1.0;

                float2 shiftedUv = input.uv - center;

                float precompute = blurWidth * (1.0 / float(numSamples - 1));

                float4 color;
                //This draws multiple images of varying scales on top of each other from the camera's texture to get a blurring effect
                for (int i = 0; i < numSamples; i++)
                {
                    float scale = blurStart + (float(i) * precompute);
                    color += tex2D(_MainTex, shiftedUv * scale + center);
                }
                color /= float(numSamples);

                // color = float4(1,0,0,1);
                return color;
            }
            ENDCG
        }
    }
}