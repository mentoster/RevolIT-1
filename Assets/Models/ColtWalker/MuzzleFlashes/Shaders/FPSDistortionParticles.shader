Shader "ColtMuzzle/FPS_Pack/Distortion" {
Properties {
		[HDR]_TintColor("Tint Color", Color) = (0,0,0,1)
		_BaseTex("Base (RGB) Gloss (A)", 2D) = "black" {}
		[HDR]_MainColor("Main Color", Color) = (1,1,1,1)
        _MainTex ("Normalmap & CutOut", 2D) = "black" {}
		_BumpAmt ("Distortion", Float) = 1
		_InvFade ("Soft Particles Factor", Float) = 0.5
}



	SubShader {

		Tags{ "Queue" = "Transparent-5" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off

		Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#pragma multi_compile_particles
#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
	fixed4 color : COLOR;
};

struct v2f {
	float4 vertex : POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvbump : TEXCOORD1;
	fixed4 color : COLOR;

		float4 projPos : TEXCOORD3;


};

sampler2D _MainTex;
sampler2D _BaseTex;
half4 _TintColor;
half4 _MainColor;
float _BumpAmt;
UNITY_DECLARE_TEX2DARRAY(_CameraDepthTexture);
float4 _DepthPyramidScale;
UNITY_DECLARE_TEX2DARRAY(_ColorPyramidTexture);
float4 _ColorPyramidTexture_TexelSize;



float4 _MainTex_ST;

	v2f vert (appdata_t v)
	{
		v2f o;

		o.vertex = UnityObjectToClipPos(v.vertex);


			o.projPos = ComputeScreenPos (o.vertex);
			o.projPos.xy *= _DepthPyramidScale.xy;
			COMPUTE_EYEDEPTH(o.projPos.z);

		o.color = v.color;
		#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
		#else
		float scale = 1.0;
		#endif
		o.uvgrab = ComputeGrabScreenPos(o.vertex);

		o.uvbump = TRANSFORM_TEX( v.texcoord, _MainTex );

		return o;
	}

	float _InvFade;

	half4 frag( v2f i ) : COLOR
	{
			float z = (UNITY_SAMPLE_TEX2DARRAY_LOD(_CameraDepthTexture, float4(i.projPos.xy / i.projPos.w, 0, 0), 0));
			float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(z));
			float partZ = i.projPos.z;
			float fade = saturate (_InvFade * (sceneZ-partZ));
			fade = _InvFade < 0.01 ? 1 : fade;
			i.color.a *= fade;


		half3 bump = UnpackNormal(tex2D( _MainTex, i.uvbump ));
		half alphaBump = saturate((0.94 - pow(bump.z, 127)) * 5);
		i.uvgrab.xy = bump.rg * i.color.a * alphaBump * _BumpAmt + i.uvgrab.xy;

		half4 grab = UNITY_SAMPLE_TEX2DARRAY_LOD(_ColorPyramidTexture, float4(i.uvgrab.xy / i.uvgrab.w, 0, 0), 0);
		grab *= _MainColor;

		grab.a = saturate(grab.a * alphaBump);
		return grab;
	}
	ENDCG
		}
	}


}
