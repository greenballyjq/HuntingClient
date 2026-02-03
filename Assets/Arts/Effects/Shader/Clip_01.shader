// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shader/AutoUV_01_Add"
{
	Properties
	{
		_MainCor("MainCor", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		[Toggle]_MainTexAlpha("MainTexAlpha", Float) = 0
		_MainTexRotat("MainTexRotat", Range( 0 , 2)) = 0
		_MainTexU("MainTexU", Float) = 0
		_MainTexV("MainTexV", Float) = 0
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_Noise("Noise", Float) = 0
		_NoiseU("NoiseU", Float) = 0
		_NoiseV("NoiseV", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		_MaskRotat("MaskRotat", Range( 0 , 2)) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend One One
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

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
			uniform sampler2D _Mask;
			uniform float4 _Mask_ST;
			uniform float _MaskRotat;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_color = v.color;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 appendResult20 = (float2(_MainTexU , _MainTexV));
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 appendResult42 = (float2(0.5 , 0.5));
				float cos39 = cos( ( _MainTexRotat * UNITY_PI ) );
				float sin39 = sin( ( _MainTexRotat * UNITY_PI ) );
				float2 rotator39 = mul( uv_MainTex - appendResult42 , float2x2( cos39 , -sin39 , sin39 , cos39 )) + appendResult42;
				float2 panner17 = ( 1.0 * _Time.y * appendResult20 + rotator39);
				float2 appendResult26 = (float2(_NoiseU , _NoiseV));
				float2 uv_NoiseTex = i.ase_texcoord1.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				float2 panner23 = ( 1.0 * _Time.y * appendResult26 + uv_NoiseTex);
				float4 tex2DNode10 = tex2D( _MainTex, ( panner17 + ( _Noise * tex2D( _NoiseTex, panner23 ).r ) ) );
				float lerpResult47 = lerp( tex2DNode10.r , tex2DNode10.a , _MainTexAlpha);
				float2 uv_Mask = i.ase_texcoord1.xy * _Mask_ST.xy + _Mask_ST.zw;
				float2 appendResult36 = (float2(0.5 , 0.5));
				float cos33 = cos( ( _MaskRotat * UNITY_PI ) );
				float sin33 = sin( ( _MaskRotat * UNITY_PI ) );
				float2 rotator33 = mul( uv_Mask - appendResult36 , float2x2( cos33 , -sin33 , sin33 , cos33 )) + appendResult36;
				
				
				finalColor = ( ( i.ase_color * _MainCor * lerpResult47 ) * tex2D( _Mask, rotator33 ).r );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.RangedFloatNode;24;-2045.582,372.9454;Inherit;False;Property;_NoiseU;NoiseU;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2053.582,457.9454;Inherit;False;Property;_NoiseV;NoiseV;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;22;-1990.582,209.9454;Inherit;False;0;21;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;26;-1859.582,373.9454;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2494.245,-189.3322;Inherit;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2491.245,-121.3322;Inherit;False;Constant;_Float3;Float 3;10;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-2578.245,-3.332214;Inherit;False;Property;_MainTexRotat;MainTexRotat;4;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;23;-1616.582,255.9454;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1725.582,-46.05463;Inherit;False;Property;_MainTexU;MainTexU;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1719.582,27.94537;Inherit;False;Property;_MainTexV;MainTexV;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-2415.582,-318.0546;Inherit;False;0;10;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;42;-2254.245,-189.3322;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PiNode;45;-2203.713,-34.02302;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;21;-1353.582,250.9454;Inherit;True;Property;_NoiseTex;NoiseTex;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-1236.582,137.9454;Inherit;False;Property;_Noise;Noise;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-1516.582,-29.05463;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;39;-1924.245,-281.3322;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;17;-1265.582,-138.0546;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1034.582,188.9454;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1124.539,516.1856;Inherit;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1120.539,608.1856;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1171.539,739.1856;Inherit;False;Property;_MaskRotat;MaskRotat;12;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-841.5815,89.94537;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;32;-901.5392,355.1856;Inherit;False;0;30;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;36;-882.5392,506.1856;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PiNode;38;-844.5392,721.1856;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-680.5,10;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;-578.8871,219.0266;Inherit;False;Property;_MainTexAlpha;MainTexAlpha;3;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;12;-585.5815,-403.0546;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-621.5346,-201.9678;Inherit;False;Property;_MainCor;MainCor;1;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;33;-542.5392,434.1856;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;47;-272.8871,-2.973389;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-249.5392,366.1856;Inherit;True;Property;_Mask;Mask;11;0;Create;True;0;0;0;False;0;False;-1;bd3197e2538ad354aacc1c4c27906a1c;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-101.0515,-295.9664;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;238.9381,-61.77634;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;60;633,-125;Float;False;True;-1;2;ASEMaterialInspector;100;5;Shader/AutoUV_01_Add;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;4;1;False;;1;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;2;RenderType=Opaque=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
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
WireConnection;17;0;39;0
WireConnection;17;2;20;0
WireConnection;27;0;28;0
WireConnection;27;1;21;1
WireConnection;29;0;17;0
WireConnection;29;1;27;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;38;0;37;0
WireConnection;10;1;29;0
WireConnection;33;0;32;0
WireConnection;33;1;36;0
WireConnection;33;2;38;0
WireConnection;47;0;10;1
WireConnection;47;1;10;4
WireConnection;47;2;48;0
WireConnection;30;1;33;0
WireConnection;13;0;12;0
WireConnection;13;1;11;0
WireConnection;13;2;47;0
WireConnection;31;0;13;0
WireConnection;31;1;30;1
WireConnection;60;0;31;0
ASEEND*/
//CHKSM=E4A6C62199A9C78F68DC02FAD41F0E1ACA95354D