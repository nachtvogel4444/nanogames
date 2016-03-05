// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

#define PI 3.14159265359

in vec2 FragmentTextureCoordinate;
in vec4 FragmentColor;

out vec4 OutputColor;

void main()
{
	float d = length(FragmentTextureCoordinate);
	float i;
	if (d > 1)
	{
		i = 0;
	}
	else
	{
		i = cos(d * PI) * 0.5 + 0.5;
	}

	OutputColor = FragmentColor * i;
}
