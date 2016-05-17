// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

layout(location = 0) uniform sampler2D ScreenTexture;

in vec2 FragmentTextureCoordinate;

out vec4 OutputColor;

vec4 fetch(vec2 coord)
{
	return (1.0 +  0.2 * sin(200 * 2 * 3.14159265359 * coord.y)) * texture2D(ScreenTexture, coord);
}

void main()
{
	float r = 5.0; // The tube radius, measured in diagonals. A smaller radius means a more curved tube.

	/* Project the view ray onto the spherical tube and compute the screen coordinates that we hit. */
	float d = (r + 1) / r;
	float xs = 0.847998304 * d * sqrt(FragmentTextureCoordinate.x * FragmentTextureCoordinate.x + 0.390625 * FragmentTextureCoordinate.y * FragmentTextureCoordinate.y);
	float a = d / xs;
	float xt = (a * d - sqrt(a * a - d * d + 1)) / (a * a + 1);
	float f = xt / xs * d * r;
	vec2 textureCoordinate = FragmentTextureCoordinate * f * 0.5 + vec2(0.5, 0.5);

	float cr = fetch(textureCoordinate + 0.001 * vec2(-0.988, 0.154)).r;
	float cg = fetch(textureCoordinate + 0.001 * vec2(0, -1)).g;
	float cb = fetch(textureCoordinate + 0.001 * vec2(0.988, 0.154)).b;

	/* Sample the input at slight offsets to simulate a slight chromatic aberration. */
	OutputColor = vec4(
		0.94 * cr + 0.03 * cg + 0.03 * cb,
		0.03 * cr + 0.94 * cg + 0.03 * cb,
		0.03 * cr + 0.03 * cg + 0.94 * cb,
		1);
}
