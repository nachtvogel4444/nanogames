// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

layout(location = 1) uniform vec2 BlurAxis;

layout(location = 0) in vec2 VertexPosition;
layout(location = 1) in vec2 VertexTextureCoordinate;

out vec2 FragmentTextureCoordinate0;
out vec2 FragmentTextureCoordinate1;
out vec2 FragmentTextureCoordinate2;
out vec2 FragmentTextureCoordinate3;
out vec2 FragmentTextureCoordinate4;
out vec2 FragmentTextureCoordinate5;
out vec2 FragmentTextureCoordinate6;
out vec2 FragmentTextureCoordinate7;

void main()
{
	gl_Position = vec4(VertexPosition, 0.0, 1.0);

	vec2 t = vec2(0.5, 0.5) + 0.5 * VertexPosition;

	/* These values result from the inverse error function and are designed to sample
	 * a gaussian distribution at equal weight intervals.
	 */

	FragmentTextureCoordinate0 = t - 1.085 * BlurAxis;
	FragmentTextureCoordinate1 = t - 0.627 * BlurAxis;
	FragmentTextureCoordinate2 = t - 0.356 * BlurAxis;
	FragmentTextureCoordinate3 = t - 0.111 * BlurAxis;
	FragmentTextureCoordinate4 = t + 0.111 * BlurAxis;
	FragmentTextureCoordinate5 = t + 0.346 * BlurAxis;
	FragmentTextureCoordinate6 = t + 0.627 * BlurAxis;
	FragmentTextureCoordinate7 = t + 1.085 * BlurAxis;
}
