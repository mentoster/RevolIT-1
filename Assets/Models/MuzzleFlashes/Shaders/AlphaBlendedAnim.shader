Shader "ColtMuzzle/FPS_Pack/AlphaBlendedAnim" {
	Properties{
		[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(0.01,5)) = 1.0
	}

		Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
		Blend SrcAlpha OneMinusSrcAlpha

		Cull Off
		ZWrite Off

		SubShader{
		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#pragma multi_compile_fog

#include "UnityCG.cginc"


		sampler2D RFX4_PointLightAttenuation;
		half4 RFX4_AmbientColor;
		float4 RFX4_LightPositions[40];
		float4 RFX4_LightColors[40];
		int RFX4_LightCount;


		sampler2D _MainTex;
		float4 _TintColor;
		float4 _MainTex_ST;
		float4 _DepthPyramidScale;
		UNITY_DECLARE_TEX2DARRAY(_CameraDepthTexture);
		float _InvFade;


	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float4 texcoords : TEXCOORD0;
		float texcoordBlend : TEXCOORD1;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float2 texcoord2 : TEXCOORD1;
		fixed blend : TEXCOORD2;
		UNITY_FOG_COORDS(3)

			float4 projPos : TEXCOORD4;

		UNITY_VERTEX_OUTPUT_STEREO
	};



	half3 ShadeCustomLights(float4 vertex, half3 normal, int lightCount)
	{
		float3 worldPos = mul(unity_ObjectToWorld, vertex);
		float3 worldNormal = UnityObjectToWorldNormal(normal);

		float3 lightColor = RFX4_AmbientColor.xyz;
		for (int i = 0; i < lightCount; i++) {
			float3 lightDir = RFX4_LightPositions[i].xyz - worldPos.xyz * RFX4_LightColors[i].w;
			half normalizedDist = length(lightDir) / RFX4_LightPositions[i].w;
			fixed attenuation = tex2Dlod(RFX4_PointLightAttenuation, half4(normalizedDist.xx, 0, 0));
			attenuation = lerp(1, attenuation, RFX4_LightColors[i].w);
			float diff = abs(dot(normalize(worldNormal), normalize(lightDir)));
			lightColor += RFX4_LightColors[i].rgb * (diff * attenuation);
		}
		return (lightColor);
	}

	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);

		o.projPos = ComputeScreenPos(o.vertex);
		o.projPos.xy *= _DepthPyramidScale.xy;
		COMPUTE_EYEDEPTH(o.projPos.z);

		o.color = v.color * _TintColor;
		o.color.rgb *= saturate(ShadeCustomLights(v.vertex, float3(0, 1, 0), RFX4_LightCount));

		o.texcoord = TRANSFORM_TEX(v.texcoords.xy, _MainTex);
		o.texcoord2 = TRANSFORM_TEX(v.texcoords.zw, _MainTex);
		o.blend = v.texcoordBlend;
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}


	fixed4 frag(v2f i) : SV_Target
	{


		float z = (UNITY_SAMPLE_TEX2DARRAY_LOD(_CameraDepthTexture, float4(i.projPos.xy / i.projPos.w, 0, 0), 0));
		float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(z));
		float partZ = i.projPos.z;
		float fade = saturate(_InvFade * (sceneZ - partZ));

		fade = _InvFade < 0.02f ? 1 : fade;
		i.color.a *= fade;


		fixed4 colA = tex2D(_MainTex, i.texcoord);
		fixed4 colB = tex2D(_MainTex, i.texcoord2);
		fixed4 col = 2.0f * i.color * lerp(colA, colB, i.blend);
		UNITY_APPLY_FOG(i.fogCoord, col);
		col.a = saturate(col.a);
		return col;
	}
		ENDCG
	}
	}
		}
}
