Shader "Devutrino/ProceduralTerrain"
{
    Properties
    {
        [KeywordEnum(SimplexNoise,PerlinNoise)] _Noise("NoiseType", int) = 0
        _NoiseSmoothness("NoiseSmoothness", Float) = 100.0
        _HeightMultiplier("Height Multiplier", Float) = 1.0
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Base Color", color) = (1,1,1,1)
        _Smoothness("Smoothness", Range(0,1)) = 0
        _Metallic("Metallic", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }
        LOD 100
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _NOISE_SIMPLEXNOISE _NOISE_PERLINNOISE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _ADDITIONAL_LIGHT_SHADOWS _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"            
            #include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise2D.hlsl"
            #include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 4);
                float perlinHeight : TEXCOORD5;
                float4 shadowCoord : TEXCOORD6; // Shadow coordinates
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _BaseColor;
            float _Smoothness, _Metallic, _HeightMultiplier, _NoiseSmoothness;

            v2f vert (VertexInput v)
            {
                v2f o;
                o.positionWS = TransformObjectToWorld(v.vertex.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normal.xyz);
                o.viewDir = normalize(_WorldSpaceCameraPos - o.positionWS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float height = 0.0f;
                #ifdef _NOISE_SIMPLEXNOISE
                    height = SimplexNoise(o.positionWS.xz/_NoiseSmoothness); // This is a hyperparameter
                #else
                    height = ClassicNoise(o.positionWS.xz/_NoiseSmoothness); // This is a hyperparameter
                #endif
                o.perlinHeight = height;
                o.positionWS.y += height * _HeightMultiplier;
                o.vertex = TransformWorldToHClip(o.positionWS);

                OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUV );
                OUTPUT_SH(o.normalWS.xyz, o.vertexSH );

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                InputData inputdata = (InputData)0;
                inputdata.positionWS = i.positionWS;
                inputdata.normalWS = normalize(i.normalWS);
                inputdata.viewDirectionWS = i.viewDir;
                inputdata.bakedGI = SAMPLE_GI( i.lightmapUV, i.vertexSH, inputdata.normalWS );
                inputdata.shadowCoord = TransformWorldToShadowCoord(i.positionWS); //URP will handle the rest! :)

                SurfaceData surfacedata;
                surfacedata.albedo = _BaseColor;
                surfacedata.specular = 0;
                surfacedata.metallic = _Metallic;
                surfacedata.smoothness = _Smoothness;
                surfacedata.normalTS = 0;
                surfacedata.emission = 0;
                surfacedata.occlusion = 1;
                surfacedata.alpha = 0;
                surfacedata.clearCoatMask = 0;
                surfacedata.clearCoatSmoothness = 0;

                return UniversalFragmentPBR(inputdata, surfacedata);
            }
            ENDHLSL
        }
    }
}
