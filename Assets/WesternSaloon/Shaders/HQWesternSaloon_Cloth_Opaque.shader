// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

/// Mario Lelas - opaque version

Shader "HQWesternSaloon/HQWesternSaloon_Cloth_Opaque" 
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_AlphaCutout("Alpha Cutout", Range(0, 1)) = 0.5
		_SpecularRGBGlossA("Specular (RGB) Gloss (A)", 2D) = "black" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		_AOPower("AO Power", Range(0, 3)) = 1
		_LightColor("Light Color", Color) = (0.6911765,0.6139273,0.4980536,1)
		[HideInInspector]_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader
		{
			/// setup tags for opaque
			Tags
			{
				"RenderType" = "Opaque" "PerformanceChecks" = "False"
			}
			Pass
			{
				Name "FORWARD"
				Tags{
				"LightMode" = "ForwardBase"
			}
			Cull Off /// needed for double sidedness

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
			#pragma exclude_renderers gles metal xbox360 ps3 psp2 
			#pragma target 3.0
			uniform half4 _Color;
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			uniform fixed _AlphaCutout;
			uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
			uniform sampler2D _SpecularRGBGlossA; uniform float4 _SpecularRGBGlossA_ST;
			uniform sampler2D _AmbientOcclusion; uniform float4 _AmbientOcclusion_ST;
			uniform float _AOPower;
			uniform half4 _LightColor;

			struct VertexInput 
			{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord0 : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
			};
			struct VertexOutput 
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				half3 normalDir : NORMAL;
				half3 tangentDir : TEXCOORD2;
				half3 bitangentDir : TEXCOORD3;
				LIGHTING_COORDS(4,5)
				UNITY_FOG_COORDS(6)
#if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
				float4 ambientOrLightmapUV : TEXCOORD7;
#endif
			};
			VertexOutput vert(VertexInput v) 
			{
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
				o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
				o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
				o.normalDir = normalize(o.normalDir);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				half3 lightColor = _LightColor0.rgb;
				o.pos = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				TRANSFER_VERTEX_TO_FRAGMENT(o)
				return o;
			}

			float4 frag(VertexOutput i, float facing : VFACE /* <- needed for double sidedness*/) : COLOR
			{
				/// removed unused variable here
				float faceSign = (facing >= 0 ? 1 : -1);
				i.normalDir *= faceSign;
				half3x3 tangentTransform = half3x3(i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
				half3 normalLocal = _NormalMap_var.rgb;
				half3 normalDirection = normalize(mul(normalLocal, tangentTransform)); // Perturbed normals
				float3 viewReflectDirection = reflect(-viewDirection, normalDirection);
				float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
				clip((_MainTex_var.a + (1.0 - (_AlphaCutout*1.499 + 0.001))) - 0.5);
				half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				half3 lightColor = _LightColor0.rgb;
				half3 halfDirection = normalize(viewDirection + lightDirection);
////// Lighting:
				half attenuation = LIGHT_ATTENUATION(i);
				half3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
				float4 _SpecularRGBGlossA_var = tex2D(_SpecularRGBGlossA,TRANSFORM_TEX(i.uv0, _SpecularRGBGlossA));
				float gloss = _SpecularRGBGlossA_var.a;
				float specPow = exp2(gloss * 10.0 + 1.0);
/////// GI Data:
				UnityLight light;
#ifdef LIGHTMAP_OFF
				light.color = lightColor;
				light.dir = lightDirection;
				light.ndotl = LambertTerm(normalDirection, light.dir);
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
				UnityGI gi = UnityGlobalIllumination(d, 1, gloss, normalDirection);
				lightDirection = gi.light.dir;
				lightColor = gi.light.color;
////// Specular:
				float NdotL = max(0, dot(normalDirection, lightDirection));
				float4 _AmbientOcclusion_var = tex2D(_AmbientOcclusion,TRANSFORM_TEX(i.uv0, _AmbientOcclusion));
				float node_9291 = pow(_AmbientOcclusion_var.r,_AOPower);
				half3 specularAO = node_9291;
				half LdotH = max(0.0,dot(lightDirection, halfDirection));
				half3 specularColor = _SpecularRGBGlossA_var.rgb;
				half specularMonochrome = max(max(specularColor.r, specularColor.g), specularColor.b);
				half NdotV = max(0.0,dot(normalDirection, viewDirection));
				half NdotH = max(0.0,dot(normalDirection, halfDirection));
				half VdotH = max(0.0,dot(viewDirection, halfDirection));
				half visTerm = SmithBeckmannVisibilityTerm(NdotL, NdotV, 1.0 - gloss);
				half normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0 - gloss)));
				half specularPBL = max(0, (NdotL*visTerm*normTerm) * unity_LightGammaCorrectionConsts_PIDiv4);
				half3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
				half grazingTerm = saturate(gloss + specularMonochrome);
				float3 indirectSpecular = (gi.indirect.specular) * specularAO;
				indirectSpecular *= FresnelLerp(specularColor, grazingTerm, NdotV);
				half3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
				NdotL = dot(normalDirection, lightDirection);
				half3 w = _LightColor.rgb*0.5; // Light wrapping
				half3 NdotLWrap = NdotL * (1.0 - w);
				half3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w);
				NdotL = max(0.0,dot(normalDirection, lightDirection));
				half fd90 = 0.5 + 2 * LdotH * LdotH * (1 - gloss);
				NdotLWrap = max(float3(0,0,0), NdotLWrap);
				half3 directDiffuse = (forwardLight + ((1 + (fd90 - 1)*pow((1.00001 - NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001 - NdotV), 5)) * NdotL))*(0.5 - max(w.r,max(w.g,w.b))*0.5) * attenColor;
				half3 indirectDiffuse = float3(0,0,0);
				indirectDiffuse += gi.indirect.diffuse;
				indirectDiffuse *= node_9291; // Diffuse AO
				half3 diffuseColor = (_MainTex_var.rgb*_Color.rgb);

				diffuseColor *= 1 - specularMonochrome;
				half3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
				half3 finalColor = diffuse + specular;
				fixed4 finalRGBA = fixed4(finalColor,1);
				UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
				return float4(finalRGBA.xyz, _Color.a);
			}
		ENDCG
	}
		Pass
		{
			Name "FORWARD_DELTA"
			Tags
			{
				"LightMode" = "ForwardAdd"
			}
			Blend One One
			Cull Off /// <- needed for double sidedness


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
			#pragma exclude_renderers gles metal xbox360 ps3 psp2 
			#pragma target 3.0
			uniform half4 _Color;
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			uniform fixed _AlphaCutout;
			uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
			uniform sampler2D _SpecularRGBGlossA; uniform float4 _SpecularRGBGlossA_ST;
			uniform half4 _LightColor;

			struct VertexInput 
			{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord0 : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				half3 normalDir : NORMAL;
				half3 tangentDir : TEXCOORD2;
				half3 bitangentDir : TEXCOORD3;
				LIGHTING_COORDS(4,5)
					UNITY_FOG_COORDS(6)
			};

			VertexOutput vert(VertexInput v) 
			{
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
				o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
				o.normalDir = normalize(o.normalDir);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				half3 lightColor = _LightColor0.rgb;
				o.pos = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				TRANSFER_VERTEX_TO_FRAGMENT(o)
				return o;
			}

			float4 frag(VertexOutput i, float facing : VFACE /* <- needed for double sidedness*/) : COLOR
			{
				float faceSign = (facing >= 0 ? 1 : -1);
				i.normalDir *= faceSign;
				half3x3 tangentTransform = half3x3(i.tangentDir, i.bitangentDir, i.normalDir);
	/////// Vectors:
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
				half3 normalLocal = _NormalMap_var.rgb;
				half3 normalDirection = normalize(mul(normalLocal, tangentTransform)); // Perturbed normals
				float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
				clip((_MainTex_var.a + (1.0 - (_AlphaCutout*1.499 + 0.001))) - 0.5);
				half3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
				half3 lightColor = _LightColor0.rgb;
				half3 halfDirection = normalize(viewDirection + lightDirection);
	////// Lighting:
				half attenuation = LIGHT_ATTENUATION(i);
				half3 attenColor = attenuation * _LightColor0.xyz;
	///////// Gloss:
				float4 _SpecularRGBGlossA_var = tex2D(_SpecularRGBGlossA,TRANSFORM_TEX(i.uv0, _SpecularRGBGlossA));
				float gloss = _SpecularRGBGlossA_var.a;
				float specPow = exp2(gloss * 10.0 + 1.0);
	////// Specular:
				float NdotL = max(0, dot(normalDirection, lightDirection));
				half LdotH = max(0.0,dot(lightDirection, halfDirection));
				half3 specularColor = _SpecularRGBGlossA_var.rgb;
				half specularMonochrome = max(max(specularColor.r, specularColor.g), specularColor.b);
				half NdotV = max(0.0,dot(normalDirection, viewDirection));
				half NdotH = max(0.0,dot(normalDirection, halfDirection));
				half VdotH = max(0.0,dot(viewDirection, halfDirection));
				half visTerm = SmithBeckmannVisibilityTerm(NdotL, NdotV, 1.0 - gloss);
				half normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0 - gloss)));
				half specularPBL = max(0, (NdotL*visTerm*normTerm) * unity_LightGammaCorrectionConsts_PIDiv4);
				half3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
				half3 specular = directSpecular;
	/////// Diffuse:
				NdotL = dot(normalDirection, lightDirection);
				half3 w = _LightColor.rgb*0.5; // Light wrapping
				half3 NdotLWrap = NdotL * (1.0 - w);
				half3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w);
				NdotL = max(0.0,dot(normalDirection, lightDirection));
				half fd90 = 0.5 + 2 * LdotH * LdotH * (1 - gloss);
				NdotLWrap = max(float3(0,0,0), NdotLWrap);
				half3 directDiffuse = (forwardLight + ((1 + (fd90 - 1)*pow((1.00001 - NdotLWrap), 5)) * (1 + (fd90 - 1)*pow((1.00001 - NdotV), 5)) * NdotL))*(0.5 - max(w.r,max(w.g,w.b))*0.5) * attenColor;
				half3 diffuseColor = (_MainTex_var.rgb*_Color.rgb);
				diffuseColor *= 1 - specularMonochrome;
				half3 diffuse = directDiffuse * diffuseColor;
	/// Final Color:
				half3 finalColor = diffuse + specular;
				fixed4 finalRGBA = fixed4(finalColor,0);
				UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
				return float4(finalRGBA.xyz, _Color.a);
			}
			ENDCG
		}
		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}
			Offset 1, 1
			Cull Off  /// <- needed for double sidedness


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_SHADOWCASTER
			#define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
			#define _GLOSSYENV 1
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fog
			#pragma exclude_renderers gles metal xbox360 ps3 psp2 
			#pragma target 3.0
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			uniform fixed _AlphaCutout;
			struct VertexInput 
			{
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
			};
			struct VertexOutput 
			{
				V2F_SHADOW_CASTER;
				float2 uv0 : TEXCOORD1;
				float4 posWorld : TEXCOORD2;
			};
			VertexOutput vert(VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}
			float4 frag(VertexOutput i/* removed unused parameter */) : COLOR
			{
	/////// Vectors:
				float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
				clip((_MainTex_var.a + (1.0 - (_AlphaCutout*1.499 + 0.001))) - 0.5);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
		Pass
		{
			Name "Meta"
			Tags
			{
				"LightMode" = "Meta"
			}
			Cull Off // <- needed for double sidedness

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
			#pragma exclude_renderers gles metal xbox360 ps3 psp2 
			#pragma target 3.0
			uniform half4 _Color;
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			uniform sampler2D _SpecularRGBGlossA; uniform float4 _SpecularRGBGlossA_ST;
			struct VertexInput 
			{
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
			VertexOutput vert(VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
				return o;
			}
			float4 frag(VertexOutput i/* removed unused parameter*/ ) : SV_Target
			{
	/////// Vectors:
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				UnityMetaInput o;
				UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

				o.Emission = 0;

				float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
				float3 diffColor = (_MainTex_var.rgb*_Color.rgb);
				float4 _SpecularRGBGlossA_var = tex2D(_SpecularRGBGlossA,TRANSFORM_TEX(i.uv0, _SpecularRGBGlossA));
				float3 specColor = _SpecularRGBGlossA_var.rgb;
				half specularMonochrome = max(max(specColor.r, specColor.g),specColor.b);
				diffColor *= (1.0 - specularMonochrome);
				float roughness = 1.0 - _SpecularRGBGlossA_var.a;
				o.Albedo = diffColor + specColor * roughness * roughness * 0.5;

				return UnityMetaFragment(o);
			}
			ENDCG
		}
	}
		FallBack "Diffuse"
}
