// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

layout(location = 0) uniform sampler2D BlurTexture;
layout(location = 1) uniform sampler2D ScreenTexture;

in vec2 FragmentTextureCoordinate;

out vec4 OutputColor;

float matrixTexture(float y)
{
	return 0.9 + 0.1 * sin(200 * 2 * 3.14159265359 * y);
}

void main()
{
	vec2 textureCoordinate = FragmentTextureCoordinate * 0.5 + vec2(0.5, 0.5);
	float m = matrixTexture(textureCoordinate.y);
	vec3 c = 2.0 * texture2D(ScreenTexture, textureCoordinate).rgb;
	OutputColor = vec4(m * c, 1);
}
