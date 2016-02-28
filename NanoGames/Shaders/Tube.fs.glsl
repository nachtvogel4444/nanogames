// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

layout(location = 0) uniform sampler2D ScreenTexture;

in vec2 FragmentTextureCoordinate;

out vec4 OutputColor;

void main()
{
	float r = 5.0; // The tube radius, measured in diagonals. A smaller radius means a more curved tube.

	/* Project the view ray onto the spherical tube and compute the screen coordinates that we hit. */
	float d = (r + 1) / r;
	float xs = 0.87157553712 * d * sqrt(FragmentTextureCoordinate.x * FragmentTextureCoordinate.x + 0.31640625 * FragmentTextureCoordinate.y * FragmentTextureCoordinate.y);
	float a = d / xs;
	float xt = (a * d - sqrt(a * a - d * d + 1)) / (a * a + 1);
	float f = xt / xs * d * r;
	vec2 textureCoordinate = FragmentTextureCoordinate * f * 0.5 + vec2(0.5, 0.5);

	/* Sample the input at slight offsets to simulate a slight chromatic aberration. */
	OutputColor = vec4(
		texture2D(ScreenTexture, textureCoordinate + 0.001 * vec2(-0.988, 0.154)).r,
		texture2D(ScreenTexture, textureCoordinate + 0.001 * vec2(0, -1)).g,
		texture2D(ScreenTexture, textureCoordinate + 0.001 * vec2(0.988, 0.154)).b,
		1);
}
