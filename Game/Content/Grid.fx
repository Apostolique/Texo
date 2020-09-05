#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler TextureSampler : register(s0);
float2 ViewportSize;
float2 LineSize;
float2 GridSize;
float4x4 ScrollMatrix;
float4 BackgroundColor;
float4 GridColor;

struct VertexToPixel {
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

float mod(float x, float m) {
    if (m == 0) {
        return x;
    }
    return (x % m + m) % m;
}

VertexToPixel SpriteVertexShader(float4 color : COLOR0, float4 texCoord : TEXCOORD0, float4 position : POSITION0) {
    VertexToPixel Output = (VertexToPixel)0;

    // Half pixel offset for correct texel centering. - This is solved by DX10 and half pixel offset would actually mess it up
    //position.xy -= 0.5;

    // Viewport adjustment.
    position.xy /= ViewportSize;
    position.xy *= float2(2, -2);
    position.xy -= float2(1, -1);

    // Transform our texture coordinates to account for camera
    texCoord = mul(texCoord, ScrollMatrix);

    texCoord.x /= GridSize.x;
    texCoord.y /= GridSize.y;

    //pass position and color to PS
    Output.Position = position;
    Output.Color = color;
    Output.TexCoord = texCoord;

    return Output;
}

float4 SpritePixelShader(VertexToPixel PSIn): COLOR0 {
    float4 color;
    float x = PSIn.TexCoord.x;
    float y = PSIn.TexCoord.y;

    float width = LineSize.x / GridSize.x;
    float height = LineSize.y / GridSize.y;

    if (mod(x + width / 2, 1) <= width || mod(y + height / 2, 1) <= height) {
        color = GridColor;
    } else {
        color = BackgroundColor;
    }
    return color;
}

technique SpriteBatch {
    pass {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}
