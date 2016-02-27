// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

in vec2 FragmentTextureCoordinate;
in float FragmentLineLength;
in vec4 FragmentColor;

float clampPoint(float s)
{
	return clamp(s, 0, FragmentLineLength);
}

void main()
{
	/* Compute the integral of a disc-shaped beam at the current pixel. */

	float d = sqrt(1 - FragmentTextureCoordinate.y * FragmentTextureCoordinate.y);
	float s = clampPoint(FragmentTextureCoordinate.x - d);
	float t = clampPoint(FragmentTextureCoordinate.x + d);

	gl_FragColor = FragmentColor * 0.5 * (t - s);
}
