Shader "UI/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

            // 描边属性
            _OutlineColor("Outline Color", Color) = (1,1,1,1)
            _OutlineWidth("Outline Width", Range(0, 10)) = 2
            _OutlineOnly("Outline Only", Range(0, 1)) = 0

            _StencilComp("Stencil Comparison", Float) = 8
            _Stencil("Stencil ID", Float) = 0
            _StencilOp("Stencil Operation", Float) = 0
            _StencilWriteMask("Stencil Write Mask", Float) = 255
            _StencilReadMask("Stencil Read Mask", Float) = 255

            _ColorMask("Color Mask", Float) = 15
            [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]

            Pass
            {
                Name "OUTLINE"

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                #pragma multi_compile __ UNITY_UI_ALPHACLIP

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                fixed4 _Color;
                fixed4 _OutlineColor;
                float _OutlineWidth;
                float _OutlineOnly;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                float4 _ClipRect;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    OUT.worldPosition = v.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                    OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    OUT.color = v.color * _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    // 采样主纹理
                    half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;

                    // 计算描边
                    float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;

                    // 检查上下左右四个方向的像素透明度
                    half4 up = tex2D(_MainTex, IN.texcoord + float2(0, texelSize.y));
                    half4 down = tex2D(_MainTex, IN.texcoord - float2(0, texelSize.y));
                    half4 left = tex2D(_MainTex, IN.texcoord - float2(texelSize.x, 0));
                    half4 right = tex2D(_MainTex, IN.texcoord + float2(texelSize.x, 0));

                    // 对角方向采样以获得更好的效果
                    half4 upLeft = tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, texelSize.y));
                    half4 upRight = tex2D(_MainTex, IN.texcoord + float2(texelSize.x, texelSize.y));
                    half4 downLeft = tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, -texelSize.y));
                    half4 downRight = tex2D(_MainTex, IN.texcoord + float2(texelSize.x, -texelSize.y));

                    // 计算边缘检测
                    half edge = up.a + down.a + left.a + right.a +
                               upLeft.a + upRight.a + downLeft.a + downRight.a;

                    // 当前像素的透明度
                    half currentAlpha = color.a;

                    // 如果当前像素透明但周围有非透明像素，则显示描边
                    half outline = (1 - currentAlpha) * saturate(edge);

                    // 混合颜色
                    half4 finalColor;
                    if (_OutlineOnly > 0.5)
                    {
                        // 仅显示描边模式
                        finalColor = _OutlineColor;
                        finalColor.a *= outline;
                    }
                    else
                    {
                        // 正常模式：描边 + 原始内容
                        finalColor = lerp(color, _OutlineColor, outline);
                        finalColor.a = max(color.a, outline * _OutlineColor.a);
                    }

                    // UI裁剪
                    finalColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(finalColor.a - 0.001);
                    #endif

                    return finalColor;
                }
                ENDCG
            }
        }
}