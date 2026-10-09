Shader "Beltz/Belt" {
Properties { _Color("Color",Color)=(1,.94,.8,1) _OutlineColor("Outline",Color)=(.025,.03,.045,1) _Phase("Travel",Float)=0 }
SubShader { Tags {"RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline"}
Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
struct A { float4 positionOS:POSITION; float2 uv:TEXCOORD0; };
struct V { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; };
CBUFFER_START(UnityPerMaterial)
half4 _Color,_OutlineColor; float _Phase;
CBUFFER_END
V vert(A a) { V o; o.positionCS=TransformObjectToHClip(a.positionOS.xyz); o.uv=a.uv; return o; }
half4 frag(V i):SV_Target {
float edge=abs(i.uv.y*2-1); float aa=max(fwidth(edge),.035);
float alpha=1-smoothstep(1-aa,1,edge);
float groove=smoothstep(.75,.94,cos((i.uv.x-_Phase)*34));
float light=.82+.18*(1-edge)+.10*groove;
float border=smoothstep(.57-aa,.57+aa,edge);
return half4(lerp(_Color.rgb*light,_OutlineColor.rgb,border),_Color.a*alpha); }
ENDHLSL
} } }
