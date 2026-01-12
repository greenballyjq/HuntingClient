// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shader/CustomClip_Blend_DoubleFace"
{
	Properties
	{
		[HDR]_MainCor("MainCor", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		[Toggle]_MainTexAlpha("MainTexAlpha", Float) = 0
		_MainTexRotat("MainTexRotat", Range( 0 , 2)) = 0
		_MainTexU("MainTexU", Float) = 0
		_MainTexV("MainTexV", Float) = 0
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_Noise("Noise", Float) = 0
		_NoiseU("NoiseU", Float) = 0
		_NoiseV("NoiseV", Float) = 0
		_ClipTex("ClipTex", 2D) = "white" {}
		_Clip("Clip", Range( 0 , 1)) = 0.45
		_ClipRotat("ClipRotat", Range( 0 , 2)) = 0
		[Toggle]_CustomOpen("CustomOpen", Float) = 0
		_ClipU("ClipU", Float) = 0
		_ClipV("ClipV", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		_MaskRotat("MaskRotat", Range( 0 , 2)) = 0
		_Fre_Bias("Fre_Bias", Float) = 0
		_Fre_Scale("Fre_Scale", Float) = 1
		_Fre_Power("Fre_Power", Float) = 5
		[Toggle]_Fre_Open("Fre_Open", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform float _Fre_Bias;
		uniform float _Fre_Scale;
		uniform float _Fre_Power;
		uniform float _Fre_Open;
		uniform float4 _MainCor;
		uniform sampler2D _MainTex;
		uniform float _MainTexU;
		uniform float _MainTexV;
		uniform float4 _MainTex_ST;
		uniform float _MainTexRotat;
		uniform float _Noise;
		uniform sampler2D _NoiseTex;
		uniform float _NoiseU;
		uniform float _NoiseV;
		uniform float4 _NoiseTex_ST;
		uniform float _MainTexAlpha;
		uniform float _Clip;
		uniform float _CustomOpen;
		uniform sampler2D _ClipTex;
		uniform float _ClipU;
		uniform float _ClipV;
		uniform float4 _ClipTex_ST;
		uniform float _ClipRotat;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform float _MaskRotat;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV84 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode84 = ( _Fre_Bias + _Fre_Scale * pow( 1.0 - fresnelNdotV84, _Fre_Power ) );
			float lerpResult96 = lerp( 1.0 , saturate( fresnelNode84 ) , _Fre_Open);
			float2 appendResult20 = (float2(_MainTexU , _MainTexV));
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 appendResult42 = (float2(0.5 , 0.5));
			float cos39 = cos( ( _MainTexRotat * UNITY_PI ) );
			float sin39 = sin( ( _MainTexRotat * UNITY_PI ) );
			float2 rotator39 = mul( uv_MainTex - appendResult42 , float2x2( cos39 , -sin39 , sin39 , cos39 )) + appendResult42;
			float2 panner17 = ( 1.0 * _Time.y * appendResult20 + rotator39);
			float2 appendResult26 = (float2(_NoiseU , _NoiseV));
			float2 uv_NoiseTex = i.uv_texcoord * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
			float2 panner23 = ( 1.0 * _Time.y * appendResult26 + uv_NoiseTex);
			float4 tex2DNode10 = tex2D( _MainTex, ( panner17 + ( _Noise * tex2D( _NoiseTex, panner23 ).r ) ) );
			float lerpResult47 = lerp( tex2DNode10.r , tex2DNode10.a , _MainTexAlpha);
			float lerpResult65 = lerp( _Clip , i.uv2_texcoord2.x , _CustomOpen);
			float2 appendResult70 = (float2(_ClipU , _ClipV));
			float2 uv_ClipTex = i.uv_texcoord * _ClipTex_ST.xy + _ClipTex_ST.zw;
			float2 appendResult36 = (float2(0.5 , 0.5));
			float cos33 = cos( ( _ClipRotat * UNITY_PI ) );
			float sin33 = sin( ( _ClipRotat * UNITY_PI ) );
			float2 rotator33 = mul( uv_ClipTex - appendResult36 , float2x2( cos33 , -sin33 , sin33 , cos33 )) + appendResult36;
			float2 panner67 = ( 1.0 * _Time.y * appendResult70 + rotator33);
			float temp_output_62_0 = step( lerpResult65 , tex2D( _ClipTex, panner67 ).r );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float2 appendResult77 = (float2(0.5 , 0.5));
			float cos73 = cos( ( _MaskRotat * UNITY_PI ) );
			float sin73 = sin( ( _MaskRotat * UNITY_PI ) );
			float2 rotator73 = mul( uv_Mask - appendResult77 , float2x2( cos73 , -sin73 , sin73 , cos73 )) + appendResult77;
			float4 tex2DNode71 = tex2D( _Mask, rotator73 );
			o.Emission = ( lerpResult96 * ( ( i.vertexColor * _MainCor * lerpResult47 ) * temp_output_62_0 * tex2DNode71.r ) ).rgb;
			o.Alpha = ( lerpResult96 * ( ( i.vertexColor.a * _MainCor.a * lerpResult47 ) * tex2DNode71.r * temp_output_62_0 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.RangedFloatNode;24;-2045.582,372.9454;Inherit;False;Property;_NoiseU;NoiseU;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2053.582,457.9454;Inherit;False;Property;_NoiseV;NoiseV;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;22;-1990.582,209.9454;Inherit;False;0;21;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;26;-1859.582,373.9454;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2494.245,-189.3322;Inherit;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2491.245,-121.3322;Inherit;False;Constant;_Float3;Float 3;10;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-2578.245,-3.332214;Inherit;False;Property;_MainTexRotat;MainTexRotat;3;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;23;-1616.582,255.9454;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1725.582,-46.05463;Inherit;False;Property;_MainTexU;MainTexU;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1719.582,27.94537;Inherit;False;Property;_MainTexV;MainTexV;5;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-2415.582,-318.0546;Inherit;False;0;10;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;42;-2254.245,-189.3322;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PiNode;45;-2203.713,-34.02302;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-2432.17,1141.12;Inherit;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-2428.17,1233.12;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-2470.996,1399.538;Inherit;False;Property;_ClipRotat;ClipRotat;12;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;21;-1353.582,250.9454;Inherit;True;Property;_NoiseTex;NoiseTex;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-1236.582,137.9454;Inherit;False;Property;_Noise;Noise;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-1516.582,-29.05463;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;39;-1924.245,-281.3322;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;32;-2209.17,980.1185;Inherit;False;0;30;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;36;-2190.17,1131.12;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PiNode;38;-2152.171,1346.12;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-1811.343,1230.326;Inherit;False;Property;_ClipU;ClipU;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-1813.35,1307.577;Inherit;False;Property;_ClipV;ClipV;15;0;Create;True;0;0;0;False;0;False;0;-1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;17;-1265.582,-138.0546;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1034.582,188.9454;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;33;-1850.172,1059.12;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;70;-1632.763,1201.232;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-841.5815,89.94537;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-986.6055,817.6177;Inherit;False;Property;_CustomOpen;CustomOpen;13;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-1083.173,653.9731;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;-1172.232,537.6936;Inherit;False;Property;_Clip;Clip;11;0;Create;True;0;0;0;False;0;False;0.45;0.597;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;67;-1395.995,1131.004;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-163.2885,441.4648;Inherit;False;Constant;_Float4;Float 4;17;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-161.2885,510.4648;Inherit;False;Constant;_Float5;Float 5;17;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-177.2885,618.4648;Inherit;False;Property;_MaskRotat;MaskRotat;17;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-680.5,10;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;5a59459881a43d846b599027073dbc95;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;-578.8871,219.0266;Inherit;False;Property;_MainTexAlpha;MainTexAlpha;2;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;65;-725.9538,624.7354;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-1052.534,1041.282;Inherit;True;Property;_ClipTex;ClipTex;10;0;Create;True;0;0;0;False;0;False;-1;3e21907b165f99b41afe0df537e538a7;dc96f03330cdec54ba1b62c076ec1d13;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;72;-95.01,305.6469;Inherit;False;0;71;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;77;25.71149,444.4648;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PiNode;79;129.7115,659.4648;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;458.1929,-629.5639;Inherit;False;Property;_Fre_Bias;Fre_Bias;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;458.1929,-547.9358;Inherit;False;Property;_Fre_Scale;Fre_Scale;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;462.8574,-461.6432;Inherit;False;Property;_Fre_Power;Fre_Power;20;0;Create;True;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;12;-585.5815,-403.0546;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;47;-272.8871,-2.973389;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-621.5346,-201.9678;Inherit;False;Property;_MainCor;MainCor;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,0.4988381,0.3160377,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;62;-429.3098,764.7154;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;73;258.192,361.5771;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FresnelNode;84;866.3335,-592.2484;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;71;516.2258,347.0345;Inherit;True;Property;_Mask;Mask;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;74;-185.8714,225.7779;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;100.9184,-331.3111;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;238.0719,-89.52716;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;83;527.0984,742.3517;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;88;1219.667,-552.6002;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;1210.861,-341.0734;Inherit;False;Constant;_Float6;Float 6;21;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;1248.861,-216.0734;Inherit;False;Property;_Fre_Open;Fre_Open;21;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;1110.871,-13.45475;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;1131.775,201.0929;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;96;1624.861,-278.0734;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;2060.438,214.7046;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;2029.699,-65.15752;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;80;2367.907,-45.83482;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Shader/CustomClip_Blend_DoubleFace;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;26;0;24;0
WireConnection;26;1;25;0
WireConnection;23;0;22;0
WireConnection;23;2;26;0
WireConnection;42;0;40;0
WireConnection;42;1;41;0
WireConnection;45;0;43;0
WireConnection;21;1;23;0
WireConnection;20;0;18;0
WireConnection;20;1;19;0
WireConnection;39;0;16;0
WireConnection;39;1;42;0
WireConnection;39;2;45;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;38;0;94;0
WireConnection;17;0;39;0
WireConnection;17;2;20;0
WireConnection;27;0;28;0
WireConnection;27;1;21;1
WireConnection;33;0;32;0
WireConnection;33;1;36;0
WireConnection;33;2;38;0
WireConnection;70;0;68;0
WireConnection;70;1;69;0
WireConnection;29;0;17;0
WireConnection;29;1;27;0
WireConnection;67;0;33;0
WireConnection;67;2;70;0
WireConnection;10;1;29;0
WireConnection;65;0;63;0
WireConnection;65;1;64;1
WireConnection;65;2;66;0
WireConnection;30;1;67;0
WireConnection;77;0;75;0
WireConnection;77;1;76;0
WireConnection;79;0;78;0
WireConnection;47;0;10;1
WireConnection;47;1;10;4
WireConnection;47;2;48;0
WireConnection;62;0;65;0
WireConnection;62;1;30;1
WireConnection;73;0;72;0
WireConnection;73;1;77;0
WireConnection;73;2;79;0
WireConnection;84;1;85;0
WireConnection;84;2;86;0
WireConnection;84;3;87;0
WireConnection;71;1;73;0
WireConnection;74;0;62;0
WireConnection;13;0;12;0
WireConnection;13;1;11;0
WireConnection;13;2;47;0
WireConnection;81;0;12;4
WireConnection;81;1;11;4
WireConnection;81;2;47;0
WireConnection;83;0;62;0
WireConnection;88;0;84;0
WireConnection;31;0;13;0
WireConnection;31;1;74;0
WireConnection;31;2;71;1
WireConnection;82;0;81;0
WireConnection;82;1;71;1
WireConnection;82;2;83;0
WireConnection;96;0;97;0
WireConnection;96;1;88;0
WireConnection;96;2;98;0
WireConnection;92;0;96;0
WireConnection;92;1;82;0
WireConnection;95;0;96;0
WireConnection;95;1;31;0
WireConnection;80;2;95;0
WireConnection;80;9;92;0
ASEEND*/
//CHKSM=9C5E87DE30AB9BE8C6CC31A7726A8BAEBBA14163