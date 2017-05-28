// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UI/Advanced Masked"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		//_Radius("UI Radius",float ) = 0.5
		//[Toggle(ALPHA_FLIPPED)] _AlphaFlipped ("Alpha Flipped", Float) = 0
		_AlphaMultiply ("_Alpha Add", Float) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ ALPHA_FLIPPED
			#pragma multi_compile __ FORCE_CLAMP_X
			#pragma multi_compile __ FORCE_CLAMP_Y
			#pragma multi_compile __ RED_AS_ALPHA
			//#pragma ALPHA_FLIPPED_ON ALPHA_FLIPPED_OFF FORCE_CLAMP
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				float4 uvShadow : TEXCOORD3;
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			float4x4 _Projection;
			float _AlphaMultiply;

			v2f vert(appdata_t IN)
			{
				float4 pos = mul(unity_ObjectToWorld, IN.vertex);
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = mul(UNITY_MATRIX_MVP, OUT.worldPosition);
				OUT.uvShadow = mul(_Projection, OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
				#endif
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _MaskTex;
			//float _AlphaFlipped;

			fixed4 frag(v2f IN) : SV_Target
			{
				float4 uv =  UNITY_PROJ_COORD(IN.uvShadow);
				#ifdef FORCE_CLAMP_X
				uv.x = clamp(uv.x,0,1);
				#endif
				#ifdef FORCE_CLAMP_Y
				uv.y = clamp(uv.y,0,1);
				#endif
				#ifdef RED_AS_ALPHA
				half a = tex2Dproj (_MaskTex,uv).r;
				#else
				half a = tex2Dproj (_MaskTex,uv).a;
				#endif
				#ifdef ALPHA_FLIPPED
				a = 1-a;
				#endif
				a *=_AlphaMultiply;

				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				color.a *=a;
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif
				//color = floor(IN.uvShadow*10)/10;
				//color.a = 1;
				return color;
			}
		ENDCG
		}
	}
}
