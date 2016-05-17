// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

layout(location = 0) uniform float Fade;

out vec4 OutputColor;

void main()
{
	OutputColor = vec4(0.01, 0.01, 0.01, Fade);
}
