Shader "Custom/FullscreenGaussianBlur"
{
    Properties
    {
        _Radius ("Blur Radius (pixels)", Range(0, 30)) = 8
        _Sigma  ("Gaussian Sigma", Range(0.1, 20)) = 6
        _Direction ("Blur Direction (x,y)", Vector) = (1, 0, 0, 0)
        _Intensity ("Intensity", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        ZWrite Off
        Cull Off
        Blend Off

        Pass
        {
            Name "FullscreenPass"

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Fuente del blit que URP te pasa automáticamente
            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);
            float4 _BlitTexture_TexelSize; // xy = 1/width, 1/height

            // Props
            float _Radius;
            float _Sigma;
            float4 _Direction; // xy usado
            float _Intensity;

            struct Attributes {
                uint vertexID : SV_VertexID;
            };
            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = GetFullScreenTriangleVertexPosition(IN.vertexID);
                OUT.uv = GetFullScreenTriangleTexCoord(IN.vertexID);
                return OUT;
            }

            // Peso gaussiano 1D
            float G(float x, float sigma)
            {
                // 1/(sqrt(2pi)*sigma) * exp(-(x^2)/(2*sigma^2))
                // Para normalizar luego, el factor frontal no es necesario
                return exp(- (x * x) / (2.0 * sigma * sigma));
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Dirección de muestreo en texeles
                float2 dir = normalize(_Direction.xy);
                // Si dirección ~0, por seguridad usa horizontal
                dir = (abs(dir.x)+abs(dir.y) < 1e-5) ? float2(1,0) : dir;

                float2 texelStep = float2(_BlitTexture_TexelSize.x, _BlitTexture_TexelSize.y);
                float2 stepVec = dir * texelStep;

                // Radio entero clamped
                int R = (int)clamp(_Radius, 0.0, 64.0);
                float sigma = max(_Sigma, 0.1);

                // Acumuladores
                float4 accum = 0;
                float  wsum  = 0;

                // Centro
                {
                    float w0 = G(0, sigma);
                    float4 c0 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv);
                    accum += c0 * w0;
                    wsum  += w0;
                }

                // Pares simétricos
                // Shader Model 3.0 permite bucle dinámico
                [loop]
                for (int i = 1; i <= R; i++)
                {
                    float w = G(i, sigma);

                    float2 off = stepVec * i;

                    float4 c1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + off);
                    float4 c2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv - off);

                    accum += (c1 + c2) * w;
                    wsum  += 2.0 * w;
                }

                float4 blurred = accum / max(wsum, 1e-6);

                // Mezcla con la imagen original por si quieres suavizar la intensidad
                float4 src = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv);
                float4 outCol = lerp(src, blurred, _Intensity);

                return outCol;
            }
            ENDHLSL
        }
    }
}
