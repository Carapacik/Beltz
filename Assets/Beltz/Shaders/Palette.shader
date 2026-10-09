Shader "Beltz/Palette" {
Properties { _TopLeft("Top left", Color)=(.63,.55,.82,1) _TopRight("Top right", Color)=(.63,.55,.82,1)
_MidLeft("Middle left",Color)=(.50,.30,.95,1) _MidRight("Middle right",Color)=(.78,.73,.89,1)
_BottomLeft("Bottom left",Color)=(.98,.76,.92,1) _BottomRight("Bottom right",Color)=(.87,.52,.78,1) }
SubShader { Tags { "RenderType"="Opaque" "Queue"="Background" "RenderPipeline"="UniversalPipeline" }
Pass { ZWrite Off Cull Off
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
struct A { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
struct V { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; };
CBUFFER_START(UnityPerMaterial)
half4 _TopLeft,_TopRight,_MidLeft,_MidRight,_BottomLeft,_BottomRight;
CBUFFER_END
V vert(A a) { V o; o.positionCS=TransformObjectToHClip(a.positionOS.xyz); o.uv=a.uv; return o; }
half4 frag(V i):SV_Target {
float x=smoothstep(0,1,i.uv.x), y=i.uv.y;
half3 b=lerp(_BottomLeft.rgb,_BottomRight.rgb,x), m=lerp(_MidLeft.rgb,_MidRight.rgb,x), t=lerp(_TopLeft.rgb,_TopRight.rgb,x);
half3 c=y<.5?lerp(b,m,smoothstep(0,.5,y)):lerp(m,t,smoothstep(.5,1,y));
return half4(c,1); }
ENDHLSL
} } }
