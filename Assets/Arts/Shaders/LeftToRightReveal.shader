Shader "UI/LeftCircleReveal"
{
    Properties
    {
        [PerRendererData] _MainTex("Main Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _RevealAmount("RevealAmount", Range(0, 1)) = 0
        _Softness("Softness", Range(0, 0.2)) = 0.02

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
                };

                sampler2D _MainTex;
                fixed4 _Color;
                float _RevealAmount;
                float _Softness;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color * _Color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // 圆形扩散中心：从左边界开始，垂直居中
                float2 circleCenter = float2(0.0, 0.5);

                // 计算当前圆的半径（从0到√2，确保能覆盖整个区域）
                float currentRadius = _RevealAmount * 1.5; // 1.5确保能完全覆盖

                // 计算像素到圆心的距离
                float dist = distance(i.uv, circleCenter);

                // 创建圆形遮罩
                float mask = smoothstep(currentRadius - _Softness, currentRadius + _Softness, dist);

                // 应用透明度：圆形内部显示，外部隐藏
                col.a *= (1 - mask);

                return col;
            }
            ENDCG
        }
        }
}