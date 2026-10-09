Shader "Beltz/FrostedUI"
{
    Properties
    {
        [PerRendererData] _MainTex("Mask", 2D) = "white" {}
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct Attributes { float4 vertex:POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; float4 glass:TEXCOORD1; float4 shape:TEXCOORD2; };
            struct Varyings { float4 position:SV_POSITION; half4 color:COLOR; float2 uv:TEXCOORD0; float4 screen:TEXCOORD1; float4 glass:TEXCOORD2; float radius:TEXCOORD3; float2 local:TEXCOORD4; };
            sampler2D _MainTex;
            Texture2D _CameraSortingLayerTexture;
            SamplerState blur_linear_clamp_sampler;
            float4 _ClipRect;
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.position=UnityObjectToClipPos(v.vertex); o.screen=ComputeScreenPos(o.position);
                o.uv=v.uv; o.color=v.color; o.glass=v.glass; o.radius=v.shape.x; o.local=v.vertex.xy;
                return o;
            }
            half3 Backdrop(float2 uv) { return _CameraSortingLayerTexture.Sample(blur_linear_clamp_sampler,saturate(uv)).rgb; }
            half4 frag(Varyings i):SV_Target
            {
                float2 halfSize=max(i.glass.zw,float2(1,1));
                float radius=max(i.radius,1);
                float2 q=abs(i.glass.xy)-halfSize+radius;
                float distance=length(max(q,0))+min(max(q.x,q.y),0)-radius;
                float pixel=max(fwidth(distance),.5);
                float inside=max(-distance,0);
                float bevel=1-smoothstep(0,12*pixel,inside);
                float2 nearest=max(q,0);
                float2 normal=sign(i.glass.xy)*(dot(nearest,nearest)>.001?normalize(nearest):(q.x>q.y?float2(1,0):float2(0,1)));
                float2 uv=i.screen.xy/i.screen.w;
                // One existing quarter-resolution scene capture, nine taps per surface.
                // Refraction is confined to the bevel so labels never distort.
                uv-=normal*bevel*bevel*7/_ScreenParams.xy;
                float2 stepUV=8/_ScreenParams.xy;
                half3 blurred=Backdrop(uv)*.20;
                blurred+=(Backdrop(uv+float2(stepUV.x,0))+Backdrop(uv-float2(stepUV.x,0))+
                          Backdrop(uv+float2(0,stepUV.y))+Backdrop(uv-float2(0,stepUV.y)))*.12;
                blurred+=(Backdrop(uv+stepUV)+Backdrop(uv-stepUV)+
                          Backdrop(uv+float2(stepUV.x,-stepUV.y))+Backdrop(uv+float2(-stepUV.x,stepUV.y)))*.08;
                half saturation=max(i.color.r,max(i.color.g,i.color.b))-min(i.color.r,min(i.color.g,i.color.b));
                half tint=lerp(.16,.72,saturate(saturation*1.6));
                half3 color=lerp(blurred,i.color.rgb,tint);
                float rim=1-smoothstep(.4*pixel,2.2*pixel,inside);
                float light=saturate(dot(normal,normalize(float2(-.45,1)))*.5+.5);
                color+=bevel*(light*.13-.035);
                color=lerp(color,half3(1,1,1),rim*lerp(.18,.74,light));
                // A broad, faint reflection gives large sheets a curved-glass appearance.
                float sheen=pow(saturate((i.glass.y/halfSize.y+1)*.5),5)*.045;
                color+=sheen;
                half alpha=tex2D(_MainTex,i.uv).a*i.color.a;
                #ifdef UNITY_UI_CLIP_RECT
                    alpha*=UnityGet2DClipping(i.local,_ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                    clip(alpha-.001);
                #endif
                return half4(color,alpha);
            }
            ENDCG
        }
    }
}
