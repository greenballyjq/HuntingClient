// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Animal_Shader_New"
{
	Properties
	{
		_DarkSoft("DarkSoft", Range( 0 , 1)) = 0.2005947
		_DarkRage("DarkRage", Range( -1 , 1)) = 0.04347826
		_DarkIntensity("DarkIntensity", Range( 0 , 1)) = 0.4673913
		_High_Rage("High_Rage", Range( -1 , 0)) = -0.5
		_High_Soft("High_Soft", Range( 0 , 1)) = 0
		[HDR]_MainCor("MainCor", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		[HDR]_HighCor("HighCor", Color) = (1,1,1,1)
		[HDR]_OutLintCor("OutLintCor", Color) = (0,0,0,0)
		_OutLineWidth("OutLineWidth", Range( 0 , 0.02)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 uv_MainTex = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode35 = tex2Dlod( _MainTex, float4( uv_MainTex, 0, 0.0) );
			float outlineVar = ( tex2DNode35.a * _OutLineWidth );
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = (_OutLintCor).rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _HighCor;
		uniform float _High_Rage;
		uniform float _High_Soft;
		uniform float _DarkRage;
		uniform float _DarkSoft;
		uniform float _DarkIntensity;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _MainCor;
		uniform float4 _OutLintCor;
		uniform float _OutLineWidth;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
			v.vertex.w = 1;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = i.worldNormal;
			float3 normalizeResult23 = normalize( ase_worldNormal );
			float dotResult25 = dot( ase_worldlightDir , normalizeResult23 );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 normalizeResult7 = normalize( ase_worldNormal );
			float dotResult4 = dot( ase_worldlightDir , normalizeResult7 );
			float lerpResult12 = lerp( ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) ) , ( saturate( ( ( dotResult4 + _DarkRage ) / _DarkSoft ) ) * ase_lightAtten ) , _DarkIntensity);
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode35 = tex2D( _MainTex, uv_MainTex );
			float3 temp_output_32_0 = ( ( (_HighCor).rgb * saturate( ( ( dotResult25 + _High_Rage ) / _High_Soft ) ) ) + ( ase_lightColor.rgb * lerpResult12 * (( tex2DNode35 * _MainCor )).rgb ) );
			c.rgb = temp_output_32_0;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = i.worldNormal;
			float3 normalizeResult23 = normalize( ase_worldNormal );
			float dotResult25 = dot( ase_worldlightDir , normalizeResult23 );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 normalizeResult7 = normalize( ase_worldNormal );
			float dotResult4 = dot( ase_worldlightDir , normalizeResult7 );
			float lerpResult12 = lerp( ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) ) , ( saturate( ( ( dotResult4 + _DarkRage ) / _DarkSoft ) ) * 1 ) , _DarkIntensity);
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode35 = tex2D( _MainTex, uv_MainTex );
			float3 temp_output_32_0 = ( ( (_HighCor).rgb * saturate( ( ( dotResult25 + _High_Rage ) / _High_Soft ) ) ) + ( ase_lightColor.rgb * lerpResult12 * (( tex2DNode35 * _MainCor )).rgb ) );
			o.Albedo = temp_output_32_0;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.WorldNormalVector;1;-3386.871,119.2425;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;7;-3132.154,142.981;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-3414.871,-79.75761;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;11;-2774.154,265.9808;Inherit;False;Property;_DarkRage;DarkRage;1;0;Create;True;0;0;0;False;0;False;0.04347826;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;4;-2887.671,0.3424835;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;22;-2395.484,-359.1626;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-2424.154,18.981;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2367.669,215.7811;Inherit;False;Property;_DarkSoft;DarkSoft;0;0;Create;True;0;0;0;False;0;False;0.2005947;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;15;-2506.214,354.9549;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;23;-2140.767,-335.424;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;24;-2423.484,-558.1625;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;5;-2020.826,57.88304;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;17;-2244.214,364.9549;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;20;-2314.214,609.9551;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.DotProductOpNode;25;-1896.284,-478.0626;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1834.725,-248.0959;Inherit;False;Property;_High_Rage;High_Rage;3;0;Create;True;0;0;0;False;0;False;-0.5;0;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;9;-1778.307,77.52159;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-2008.216,450.955;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;37;-1584.209,784.9702;Inherit;False;Property;_MainCor;MainCor;5;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;35;-1654.139,591.0027;Inherit;True;Property;_MainTex;MainTex;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-1505.788,-441.0941;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1612.581,-164.0342;Inherit;False;Property;_High_Soft;High_Soft;4;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1463.216,200.9549;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1523.216,491.955;Inherit;False;Property;_DarkIntensity;DarkIntensity;2;0;Create;True;0;0;0;False;0;False;0.4673913;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;21;-1804.216,490.955;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;28;-1213.581,-371.0342;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;39;-1188.209,-597.0298;Inherit;False;Property;_HighCor;HighCor;7;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1244.209,599.9702;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;12;-1143.216,309.9549;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;34;-1048.48,160.3765;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ComponentMaskNode;36;-1051.209,582.9702;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;31;-1010.581,-331.0342;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;41;-882.2095,-474.0298;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;43;-722.2095,530.9702;Inherit;False;Property;_OutLintCor;OutLintCor;8;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;45;-730.2095,845.9702;Inherit;False;Property;_OutLineWidth;OutLineWidth;9;0;Create;True;0;0;0;False;0;False;0;0;0;0.02;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-768.4805,316.3765;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-609.2095,-260.0298;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;44;-464.2095,526.9702;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-417.2095,743.9702;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-377.9809,71.39749;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OutlineNode;42;-167.2095,556.9702;Inherit;False;0;True;None;0;0;Front;True;True;True;True;0;False;;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;200,10;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Animal_Shader_New;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;1;0
WireConnection;4;0;3;0
WireConnection;4;1;7;0
WireConnection;10;0;4;0
WireConnection;10;1;11;0
WireConnection;23;0;22;0
WireConnection;5;0;10;0
WireConnection;5;1;6;0
WireConnection;17;0;15;0
WireConnection;25;0;24;0
WireConnection;25;1;23;0
WireConnection;9;0;5;0
WireConnection;18;0;17;0
WireConnection;18;1;20;2
WireConnection;26;0;25;0
WireConnection;26;1;27;0
WireConnection;16;0;9;0
WireConnection;16;1;15;0
WireConnection;21;0;18;0
WireConnection;28;0;26;0
WireConnection;28;1;29;0
WireConnection;38;0;35;0
WireConnection;38;1;37;0
WireConnection;12;0;21;0
WireConnection;12;1;16;0
WireConnection;12;2;14;0
WireConnection;36;0;38;0
WireConnection;31;0;28;0
WireConnection;41;0;39;0
WireConnection;33;0;34;1
WireConnection;33;1;12;0
WireConnection;33;2;36;0
WireConnection;40;0;41;0
WireConnection;40;1;31;0
WireConnection;44;0;43;0
WireConnection;46;0;35;4
WireConnection;46;1;45;0
WireConnection;32;0;40;0
WireConnection;32;1;33;0
WireConnection;42;0;44;0
WireConnection;42;1;46;0
WireConnection;0;0;32;0
WireConnection;0;13;32;0
WireConnection;0;11;42;0
ASEEND*/
//CHKSM=8AF08460D2B3C08E3516EC684860133D3987CC02