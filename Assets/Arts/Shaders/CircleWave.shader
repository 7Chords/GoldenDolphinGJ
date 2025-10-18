Shader "UI/CircleWave"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _CircleColor("CircleColor", Color) = (0,0,0,0.5)
        _BorderColor("BorderColor", Color) = (0,0,0,0.8)
        _BorderWidth("BorderWidth", Range(0, 0.5)) = 0.05
        _WaveSpeed("WaveSpeed", Float) = 0.5
        _StartRadius("StartRadius", Range(0, 1)) = 0.1
        _MaxRadius("MaxRadius", Range(0, 2)) = 1.5

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
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
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR;
                    float2 worldPos : TEXCOORD1;
                };

                sampler2D _MainTex;
                fixed4 _Color;
                fixed4 _CircleColor;
                fixed4 _BorderColor;
                float _BorderWidth;
                float _WaveSpeed;
                float _StartRadius;
                float _MaxRadius;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color * _Color;
                    o.worldPos = v.vertex.xy;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // 计算中心点距离 (UI的UV是从左下角开始的)
                    float2 center = float2(0.5, 0.5);
                    float dist = distance(i.uv, center);

                    // 计算当前时间下的圆半径（循环效果）
                    float currentRadius = _StartRadius + frac(_Time.y * _WaveSpeed) * (_MaxRadius - _StartRadius);

                    // 创建圆环效果
                    float circleInner = smoothstep(currentRadius - _BorderWidth - 0.02,
                                                 currentRadius - _BorderWidth + 0.02, dist);
                    float circleOuter = smoothstep(currentRadius - 0.02, currentRadius + 0.02, dist);

                    // 计算边界区域
                    float borderArea = circleOuter - circleInner;

                    // 混合颜色
                    fixed4 col = _CircleColor;
                    col = lerp(_BorderColor, col, circleInner);

                    // 圆外完全透明
                    col.a *= (1 - circleOuter);

                    // 与UI原始颜色混合
                    col *= i.color;

                    return col;
                }
                ENDCG
            }
        }
}