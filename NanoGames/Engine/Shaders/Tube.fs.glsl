// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

layout(location = 0) uniform sampler2D BlurTexture;
layout(location = 1) uniform sampler2D ScreenTexture;

in vec2 FragmentTextureCoordinate;

out vec4 OutputColor;

vec3 screenTexture(vec2 coord)
{
	return 1.5 * texture2D(ScreenTexture, coord).rgb + 2.0 * texture2D(BlurTexture, coord).rgb;
}

float matrixTexture(float y)
{
	return 0.9 + 0.1 * sin(200 * 2 * 3.14159265359 * y);
}

void main()
{
	float r = 8.0; // The tube radius, measured in diagonals. A smaller radius means a more curved tube.

	/* Project the view ray onto the spherical tube and compute the screen coordinates that we hit. */
	float d = (r + 1) / r;
	float xs = 0.847998304 * d * sqrt(FragmentTextureCoordinate.x * FragmentTextureCoordinate.x + 0.390625 * FragmentTextureCoordinate.y * FragmentTextureCoordinate.y);
	float a = d / xs;
	float xt = (a * d - sqrt(a * a - d * d + 1)) / (a * a + 1);
	float f = xt / xs * d * r;
	vec2 textureCoordinate = FragmentTextureCoordinate * f * 0.5 + vec2(0.5, 0.5);

	float cr = screenTexture(textureCoordinate + 0.0005 * vec2(0.866, 0.5)).r;
	float cg = screenTexture(textureCoordinate + 0.0005 * vec2(0, -1)).g;
	float cb = screenTexture(textureCoordinate + 0.0005 * vec2(-0.866, 0.5)).b;
	
	float m = matrixTexture(textureCoordinate.y);	

	/* Sample the input at slight offsets to simulate a slight chromatic aberration. */
	OutputColor = vec4(m * vec3(cr, cg, cb), 1);
}
