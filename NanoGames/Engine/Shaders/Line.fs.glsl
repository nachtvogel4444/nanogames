// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

#define PI 3.14159265359

in vec2 FragmentTextureCoordinate;
in float FragmentLineLength;
in vec4 FragmentColor;

out vec4 OutputColor;

float beamClamp(float x)
{
	return clamp(x, 0, FragmentLineLength);
}

float beamIntegral(float s, float t)
{
	/*
	 * Define the beam intensity b(x) = exp(-r(x)^2) = exp(-(x^2 + y^2)).
	 * This is a gaussian distribution of the intensity.
	 *
	 * To get the contribution of the beam to the current pixel, define B(x) = integral b(x) dx
	 * and evaluate B(t) - B(s).
	 *
	 * Analytically, B(X) = exp(-y^2) * erf(x) + C.
	 *
	 * We approximate exp(-y^2) with a scaled cosine.
	 * We approximate erf(x) with a scaled sine.
	 * The result is the formula B(x) = cos(y PI + 1) * sin(x PI / 2).
	 *
	 * With those approximations, both factors smoothly vanish at +/- 1,
	 * which is important so that we don't get a hard cutoff at the border.
	 */

	return 0.25 * (cos(FragmentTextureCoordinate.y * PI) + 1) * (sin(0.5 * PI * t) - sin(0.5 * PI * s));
}

void main()
{
	/* Approximate the integral of a gaussian-shaped beam at the current pixel. */

	float d = sqrt(1 - FragmentTextureCoordinate.y * FragmentTextureCoordinate.y);
	float s = beamClamp(FragmentTextureCoordinate.x - d) - FragmentTextureCoordinate.x;
	float t = beamClamp(FragmentTextureCoordinate.x + d) - FragmentTextureCoordinate.x;

	float i = beamIntegral(s, t);

	OutputColor = FragmentColor * i;
}
