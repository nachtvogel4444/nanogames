// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

layout(location = 0) in vec2 VertexPosition;

void main()
{
	gl_Position = vec4(VertexPosition, 0.0, 1.0);
}
