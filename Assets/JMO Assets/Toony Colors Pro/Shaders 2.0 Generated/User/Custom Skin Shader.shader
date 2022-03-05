// Toony Colors Pro+Mobile 2
// (c) 2014-2019 Jean Moreno

Shader "Toony Colors Pro 2/User/Custom Skin Shader"
{
	Properties
	{
	[TCP2HeaderHelp(BASE, Base Properties)]
		//TOONY COLORS
		_Color ("Color", Color) = (1,1,1,1)
		_HColor ("Highlight Color", Color) = (0.785,0.785,0.785,1.0)
		_SColor ("Shadow Color", Color) = (0.195,0.195,0.195,1.0)
		_WrapFactor ("Light Wrapping", Range(-1,3)) = 1.0

		//DIFFUSE
		_MainTex ("Main Texture", 2D) = "white" {}

		[Header(Texture Blending (Vertex Colors))]
		_BlendTex1 ("Texture 1 (r)", 2D) = "white" {}
		_BlendTex2 ("Texture 2 (g)", 2D) = "white" {}
		_BlendTex3 ("Texture 3 (b)", 2D) = "white" {}
		_DiffTint ("Diffuse Tint", Color) = (0.7,0.8,1,1)
	[TCP2Separator]

		//TOONY COLORS RAMP
		[TCP2Header(RAMP SETTINGS)]

		[TCP2Gradient] _Ramp			("Toon Ramp (RGB)", 2D) = "gray" {}
	[TCP2Separator]

	[Header(Masks)]
		[NoScaleOffset]
		_Mask1 ("Mask 1 (Specular,Roughness)", 2D) = "black" {}
		[NoScaleOffset]
		_Mask2 ("Mask 2 (Reflection)", 2D) = "black" {}
	[TCP2Separator]

	[TCP2HeaderHelp(NORMAL MAPPING, Normal Bump Map)]
		//BUMP
		_BumpMap ("Normal map (RGB)", 2D) = "bump" {}
		[NoScaleOffset] _BumpMap1 ("Normal map 1", 2D) = "bump" {}
		[NoScaleOffset] _BumpMap2 ("Normal map 2", 2D) = "bump" {}
		[NoScaleOffset] _BumpMap3 ("Normal map 3", 2D) = "bump" {}
		_BumpScale ("Scale", Float) = 1.0
	[TCP2Separator]

	[TCP2HeaderHelp(SPECULAR, Specular)]
		//SPECULAR
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Smoothness ("Roughness", Range(0,1)) = 0.5
		_SpecSmooth ("Smoothness", Range(0,1)) = 0.05
	[TCP2Separator]

	[TCP2HeaderHelp(REFLECTION, Reflection)]
		//REFLECTION
		_ReflSmoothness ("Smoothness", Range(0.0,1.0)) = 1
	[TCP2Separator]

	[TCP2HeaderHelp(RIM, Rim)]
		//RIM LIGHT
		_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
		_RimMin ("Rim Min", Range(0,2)) = 0.5
		_RimMax ("Rim Max", Range(0,2)) = 1.0
		//RIM DIRECTION
		_RimDir ("Rim Direction", Vector) = (0.0,0.0,1.0,0.0)
	[TCP2Separator]

	[TCP2HeaderHelp(TRANSPARENCY)]
		//Alpha Testing
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	[TCP2Separator]


		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{

		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom vertex:vert exclude_path:deferred exclude_path:prepass
		#pragma target 3.0

		//================================================================
		// VARIABLES

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _Mask1;
		sampler2D _Mask2;
		sampler2D _BlendTex1;
		float4 _BlendTex1_ST;
		sampler2D _BlendTex2;
		float4 _BlendTex2_ST;
		sampler2D _BlendTex3;
		float4 _BlendTex3_ST;
		fixed _ReflSmoothness;
		sampler2D _BumpMap;
		sampler2D _BumpMap1;
		sampler2D _BumpMap2;
		sampler2D _BumpMap3;
		half _BumpScale;
		fixed _Smoothness;
		fixed4 _RimColor;
		fixed _RimMin;
		fixed _RimMax;
		float4 _RimDir;
		fixed _Cutoff;

		#define UV_MAINTEX uv_MainTex

		struct Input
		{
			half2 uv_MainTex;
			half2 uv_BumpMap;
			float3 worldPos;
			float3 worldRefl;
			float3 worldNormal;
			INTERNAL_DATA
			float3 bViewDir;
			float4 color : COLOR;
		};

		//================================================================
		// CUSTOM LIGHTING

		//Lighting-related variables
		fixed4 _HColor;
		fixed4 _SColor;
		half _WrapFactor;
		sampler2D _Ramp;
		fixed _SpecSmooth;
		fixed4 _DiffTint;

		//Specular help functions (from UnityStandardBRDF.cginc)
		inline half3 SafeNormalize(half3 inVec)
		{
			half dp3 = max(0.001f, dot(inVec, inVec));
			return inVec * rsqrt(dp3);
		}


		//GGX
		#define INV_PI        0.31830988618f
		#if defined(SHADER_API_MOBILE)
			#define EPSILON 1e-4f
		#else
			#define EPSILON 1e-7f
		#endif
		inline half GGX(half NdotH, half roughness)
		{
			half a2 = roughness * roughness;
			half d = (NdotH * a2 - NdotH) * NdotH + 1.0f;
			return INV_PI * a2 / (d * d + EPSILON);
		}

		// Instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			half atten;
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
		};

		inline half4 LightingToonyColorsCustom (inout SurfaceOutputCustom s, half3 viewDir, UnityGI gi)
		{
		#define IN_NORMAL s.Normal
	
			half3 lightDir = gi.light.dir;
		#if defined(UNITY_PASS_FORWARDBASE)
			half3 lightColor = _LightColor0.rgb;
			half atten = s.atten;
		#else
			half3 lightColor = gi.light.color.rgb;
			half atten = 1;
		#endif

			IN_NORMAL = normalize(IN_NORMAL);
			fixed ndl = max(0, (dot(IN_NORMAL, lightDir) + _WrapFactor) / (1+_WrapFactor));
			#define NDL ndl


			#define		RAMP_TEXTURE	_Ramp

			fixed3 ramp = tex2D(RAMP_TEXTURE, fixed2(NDL,NDL)).rgb;
		#if !(POINT) && !(SPOT)
			ramp *= atten;
		#endif
		#if !defined(UNITY_PASS_FORWARDBASE)
			_SColor = fixed4(0,0,0,1);
		#endif
			_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
			ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);
			fixed3 wrappedLight = saturate(_DiffTint.rgb + saturate(dot(IN_NORMAL, lightDir)));
			ramp *= wrappedLight;
			//Specular: GGX
			half3 halfDir = SafeNormalize(lightDir + viewDir);
			half roughness = s.Specular*s.Specular;
			half nh = saturate(dot(IN_NORMAL, halfDir));
			half spec = GGX(nh, saturate(roughness));
			spec *= UNITY_PI * 0.05;
		#ifdef UNITY_COLORSPACE_GAMMA
			spec = max(0, sqrt(max(1e-4h, spec)));
			half surfaceReduction = 1.0 - 0.28*roughness*s.Specular;
		#else
			half surfaceReduction = 1.0 / (roughness*roughness + 1.0);
		#endif
			spec = max(0, spec * ndl);
			spec *= surfaceReduction * s.Gloss;
			spec = smoothstep(0.5-_SpecSmooth*0.5, 0.5+_SpecSmooth*0.5, spec);
			spec *= atten;
			fixed4 c;
			c.rgb = s.Albedo * lightColor.rgb * ramp;
		#if (POINT || SPOT)
			c.rgb *= atten;
		#endif

			#define SPEC_COLOR	_SpecColor.rgb
			c.rgb += lightColor.rgb * SPEC_COLOR * spec;
			c.a = s.Alpha;

		#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
			c.rgb += s.Albedo * gi.indirect.diffuse;
		#endif

			return c;
		}

		void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom s, UnityGIInput data, inout UnityGI gi)
		{
			gi = UnityGlobalIllumination(data, 1.0, IN_NORMAL);

			s.atten = data.atten;	//transfer attenuation to lighting function
			gi.light.color = _LightColor0.rgb;	//remove attenuation
		}

		//Vertex input
		struct appdata_tcp2
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 tangent : TANGENT;
			fixed4 color : COLOR;
	#if UNITY_VERSION >= 550
			UNITY_VERTEX_INPUT_INSTANCE_ID
	#endif
		};

		//================================================================
		// VERTEX FUNCTION

		inline float3 TCP2_ObjSpaceViewDir( in float4 v )
		{
			float3 camPos = _WorldSpaceCameraPos;
			camPos += mul(_RimDir, UNITY_MATRIX_V).xyz;
			float3 objSpaceCameraPos = mul(unity_WorldToObject, float4(camPos, 1)).xyz;
			return objSpaceCameraPos - v.xyz;
		}

		void vert(inout appdata_tcp2 v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			TANGENT_SPACE_ROTATION;
			o.bViewDir = mul(rotation, TCP2_ObjSpaceViewDir(v.vertex));
		}

		//================================================================
		// SURFACE FUNCTION

		void surf(Input IN, inout SurfaceOutputCustom o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.UV_MAINTEX);

			//Masks
			fixed4 mask1 = tex2D(_Mask1, IN.UV_MAINTEX);
			fixed4 mask2 = tex2D(_Mask2, IN.UV_MAINTEX);

			//Texture Blending
			#define MAIN_UV IN.UV_MAINTEX
			#define BLEND_SOURCE IN.color

			fixed4 tex1 = tex2D(_BlendTex1, MAIN_UV * _BlendTex1_ST.xy + _BlendTex1_ST.zw);
			fixed4 tex2 = tex2D(_BlendTex2, MAIN_UV * _BlendTex2_ST.xy + _BlendTex2_ST.zw);
			fixed4 tex3 = tex2D(_BlendTex3, MAIN_UV * _BlendTex3_ST.xy + _BlendTex3_ST.zw);

			mainTex = lerp(mainTex, tex1, BLEND_SOURCE.r);
			mainTex = lerp(mainTex, tex2, BLEND_SOURCE.g);
			mainTex = lerp(mainTex, tex3, BLEND_SOURCE.b);
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = mainTex.a * _Color.a;
	
			//Cutout (Alpha Testing)
			clip (o.Alpha - _Cutoff);

			//Specular
			_Smoothness *= mask1.a;
			_Smoothness = 1 - _Smoothness;	//smoothness to roughness
			o.Gloss = mask1.a;
			o.Specular = _Smoothness;

			//Normal map
			half4 normalMap = tex2D(_BumpMap, IN.uv_BumpMap.xy);

			//Texture Blending (Normal maps)
			fixed4 bump1 = tex2D(_BumpMap1, IN.uv_MainTex * _BlendTex1_ST.xy + _BlendTex1_ST.zw);
			fixed4 bump2 = tex2D(_BumpMap2, IN.uv_MainTex * _BlendTex2_ST.xy + _BlendTex2_ST.zw);
			fixed4 bump3 = tex2D(_BumpMap3, IN.uv_MainTex * _BlendTex3_ST.xy + _BlendTex3_ST.zw);
			normalMap = lerp(normalMap, bump1, BLEND_SOURCE.r);
			normalMap = lerp(normalMap, bump2, BLEND_SOURCE.g);
			normalMap = lerp(normalMap, bump3, BLEND_SOURCE.b);
			o.Normal = UnpackScaleNormal(normalMap, _BumpScale);

			//Reflection
			half3 eyeVec = IN.worldPos.xyz - _WorldSpaceCameraPos.xyz;
			half3 worldNormal = reflect(eyeVec, WorldNormalVector(IN, o.Normal));
			float oneMinusRoughness = _ReflSmoothness;
			fixed3 reflColor = fixed3(0,0,0);
		#if UNITY_SPECCUBE_BOX_PROJECTION
			half3 worldNormal0 = BoxProjectedCubemapDirection (worldNormal, IN.worldPos, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);
		#else
			half3 worldNormal0 = worldNormal;
		#endif
			half3 env0 = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, worldNormal0, 1-oneMinusRoughness);

		#if UNITY_SPECCUBE_BLENDING
			const float kBlendFactor = 0.99999;
			float blendLerp = unity_SpecCube0_BoxMin.w;
			UNITY_BRANCH
			if (blendLerp < kBlendFactor)
			{
			#if UNITY_SPECCUBE_BOX_PROJECTION
				half3 worldNormal1 = BoxProjectedCubemapDirection (worldNormal, IN.worldPos, unity_SpecCube1_ProbePosition, unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax);
			#else
				half3 worldNormal1 = worldNormal;
			#endif

				half3 env1 = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1,unity_SpecCube0), unity_SpecCube1_HDR, worldNormal1, 1-oneMinusRoughness);
				reflColor = lerp(env1, env0, blendLerp);
			}
			else
			{
				reflColor = env0;
			}
		#else
			reflColor = env0;
		#endif
			reflColor *= 0.5;
			reflColor *= mask2.a;
			o.Emission += reflColor.rgb;

			//Rim
			float3 viewDir = normalize(IN.bViewDir);
			half rim = 1.0f - saturate( dot(viewDir, o.Normal) );
			rim = smoothstep(_RimMin, _RimMax, rim);
			o.Emission += (_RimColor.rgb * rim) * _RimColor.a;
		}

		ENDCG
	}

	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}
