// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "HQWesternSaloon/HQWesternSaloon_Water" {
    Properties {
        _DirtMask ("DirtMask", 2D) = "white" {}
        _NormalMapRefraction ("Normal Map (Refraction)", 2D) = "black" {}
        _WaterColor ("Water Color", Color) = (0.5147171,0.4671224,0.08327018,0.1294118)
        _DirtColor ("DirtColor", Color) = (0.397065,0.3331571,0.1956156,1)
        _Specular ("Specular", Range(0, 1)) = 0.27
        _Gloss ("Gloss", Range(0, 1)) = 0.948
        _RefractionIntensity ("Refraction Intensity", Range(0, 0.3)) = 0.1
        _DepthBlend ("Depth Blend", Range(0, 1)) = 0.075
        _SpeedDivide ("Speed Divide", Range(0, 1)) = 1
        _DirtDirection ("Dirt Direction", Range(-1, 1)) = -1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles xbox360 ps3 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float _RefractionIntensity;
            uniform sampler2D _NormalMapRefraction; uniform float4 _NormalMapRefraction_ST;
            uniform sampler2D _DirtMask; uniform float4 _DirtMask_ST;
            uniform float4 _DirtColor;
            uniform float4 _WaterColor;
            uniform float _Specular;
            uniform float _Gloss;
            uniform float _SpeedDivide;
            uniform float _DepthBlend;
            uniform float _DirtDirection;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                half3 normalDir : NORMAL;
                half3 tangentDir : TEXCOORD2;
                half3 bitangentDir : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
                float4 projPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                half3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_8433 = _Time + _TimeEditor;
                float2 node_749 = (i.uv0+(node_8433.g/(_SpeedDivide*199.0+1.0))*float2(1,1));
                half4 _NormalMapRefraction_var = tex2D(_NormalMapRefraction,TRANSFORM_TEX(node_749, _NormalMapRefraction));
                float node_3489 = saturate((sceneZ-partZ)/(_DepthBlend*0.4+0.0));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + ((_NormalMapRefraction_var.rg*(_RefractionIntensity*0.2))*node_3489);
                half4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                half3x3 tangentTransform = half3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                half3 normalLocal = lerp(half3(0,0,1),_NormalMapRefraction_var.rgb,_RefractionIntensity);
                half3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                half3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                half3 lightColor = _LightColor0.rgb;
                half3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                half3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                UnityGI gi = UnityGlobalIllumination (d, 1, gloss, normalDirection);
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                half NdotL = max(0, dot( normalDirection, lightDirection ));
                half LdotH = max(0.0,dot(lightDirection, halfDirection));
                half specularMonochrome = _Specular;
                half NdotV = max(0.0,dot( normalDirection, viewDirection ));
                half NdotH = max(0.0,dot( normalDirection, halfDirection ));
                half VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * unity_LightGammaCorrectionConsts_PIDiv4 );
                half3 specularColor = float3(_Specular,_Specular,_Specular);
                half3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                half3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                half3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                half node_29 = 1.0;
                half3 w = half3(node_29,node_29,node_29)*0.5; // Light wrapping
                half3 NdotLWrap = NdotL * ( 1.0 - w );
                half3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                half3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_29,node_29,node_29);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                NdotLWrap = max(float3(0,0,0), NdotLWrap);
                half3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*pow((1.00001-NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL))*(0.5-max(w.r,max(w.g,w.b))*0.5) * attenColor;
                half3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float4 node_1009 = _Time + _TimeEditor;
                float2 node_591 = (i.uv0+(node_1009.g/(_DirtDirection*600.0+0.0))*float2(1,1));
                float4 _DirtMask_var = tex2D(_DirtMask,TRANSFORM_TEX(node_591, _DirtMask));
                half3 diffuseColor = lerp(_WaterColor.rgb,_DirtColor.rgb,_DirtMask_var.a);
                diffuseColor *= 1-specularMonochrome;
                half3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                half3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,((_DirtMask_var.a+_WaterColor.a)*node_3489)),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles xbox360 ps3 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float _RefractionIntensity;
            uniform sampler2D _NormalMapRefraction; uniform float4 _NormalMapRefraction_ST;
            uniform sampler2D _DirtMask; uniform float4 _DirtMask_ST;
            uniform float4 _DirtColor;
            uniform float4 _WaterColor;
            uniform float _Specular;
            uniform float _Gloss;
            uniform float _SpeedDivide;
            uniform float _DepthBlend;
            uniform float _DirtDirection;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                half3 normalDir : NORMAL;
                half3 tangentDir : TEXCOORD2;
                half3 bitangentDir : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
                float4 projPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                half3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_8433 = _Time + _TimeEditor;
                float2 node_749 = (i.uv0+(node_8433.g/(_SpeedDivide*199.0+1.0))*float2(1,1));
                half4 _NormalMapRefraction_var = tex2D(_NormalMapRefraction,TRANSFORM_TEX(node_749, _NormalMapRefraction));
                float node_3489 = saturate((sceneZ-partZ)/(_DepthBlend*0.4+0.0));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + ((_NormalMapRefraction_var.rg*(_RefractionIntensity*0.2))*node_3489);
                half4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                half3x3 tangentTransform = half3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                half3 normalLocal = lerp(half3(0,0,1),_NormalMapRefraction_var.rgb,_RefractionIntensity);
                half3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                half3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                half3 lightColor = _LightColor0.rgb;
                half3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                half NdotL = max(0, dot( normalDirection, lightDirection ));
                half LdotH = max(0.0,dot(lightDirection, halfDirection));
                half specularMonochrome = _Specular;
                half NdotV = max(0.0,dot( normalDirection, viewDirection ));
                half NdotH = max(0.0,dot( normalDirection, halfDirection ));
                half VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * unity_LightGammaCorrectionConsts_PIDiv4 );
                half3 specularColor = float3(_Specular,_Specular,_Specular);
                half3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                half node_29 = 1.0;
                half3 w = half3(node_29,node_29,node_29)*0.5; // Light wrapping
                half3 NdotLWrap = NdotL * ( 1.0 - w );
                half3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                half3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_29,node_29,node_29);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                NdotLWrap = max(float3(0,0,0), NdotLWrap);
                half3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*pow((1.00001-NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL))*(0.5-max(w.r,max(w.g,w.b))*0.5) * attenColor;
                float4 node_1009 = _Time + _TimeEditor;
                float2 node_591 = (i.uv0+(node_1009.g/(_DirtDirection*600.0+0.0))*float2(1,1));
                float4 _DirtMask_var = tex2D(_DirtMask,TRANSFORM_TEX(node_591, _DirtMask));
                half3 diffuseColor = lerp(_WaterColor.rgb,_DirtColor.rgb,_DirtMask_var.a);
                diffuseColor *= 1-specularMonochrome;
                half3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                half3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * ((_DirtMask_var.a+_WaterColor.a)*node_3489),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers gles xbox360 ps3 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _DirtMask; uniform float4 _DirtMask_ST;
            uniform float4 _DirtColor;
            uniform float4 _WaterColor;
            uniform float _Specular;
            uniform float _Gloss;
            uniform float _DirtDirection;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
/////// Vectors:
                half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 node_1009 = _Time + _TimeEditor;
                float2 node_591 = (i.uv0+(node_1009.g/(_DirtDirection*600.0+0.0))*float2(1,1));
                float4 _DirtMask_var = tex2D(_DirtMask,TRANSFORM_TEX(node_591, _DirtMask));
                half3 diffColor = lerp(_WaterColor.rgb,_DirtColor.rgb,_DirtMask_var.a);
                half specularMonochrome = _Specular;
                diffColor *= (1.0-specularMonochrome);
                float roughness = 1.0 - _Gloss;
                float3 specColor = float3(_Specular,_Specular,_Specular);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
	FallBack "Standard (Specular setup)"
}
