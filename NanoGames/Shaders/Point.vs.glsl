// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

layout(location = 0) in vec2 VertexPosition;
layout(location = 1) in vec2 VertexTextureCoordinate;
layout(location = 2) in vec3 VertexColor;

out vec2 FragmentTextureCoordinate;
out vec4 FragmentColor;

void main()
{
	gl_Position = vec4(VertexPosition, 0.0, 1.0);
	FragmentTextureCoordinate = VertexTextureCoordinate;
	FragmentColor = vec4(VertexColor, 1.0);
}
