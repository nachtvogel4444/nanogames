// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

#version 430

layout(location = 0) uniform sampler2D ScreenTexture;

in vec2 FragmentTextureCoordinate0;
in vec2 FragmentTextureCoordinate1;
in vec2 FragmentTextureCoordinate2;
in vec2 FragmentTextureCoordinate3;
in vec2 FragmentTextureCoordinate4;
in vec2 FragmentTextureCoordinate5;
in vec2 FragmentTextureCoordinate6;
in vec2 FragmentTextureCoordinate7;

out vec4 OutputColor;

void main()
{
	OutputColor = 0.125 * (
		texture2D(ScreenTexture, FragmentTextureCoordinate0, 4.5)
		+ texture2D(ScreenTexture, FragmentTextureCoordinate1, 3.5)
 		+ texture2D(ScreenTexture, FragmentTextureCoordinate2, 3.5)
 		+ texture2D(ScreenTexture, FragmentTextureCoordinate3, 3.5)
 		+ texture2D(ScreenTexture, FragmentTextureCoordinate4, 3.5)
 		+ texture2D(ScreenTexture, FragmentTextureCoordinate5, 3.5)
 		+ texture2D(ScreenTexture, FragmentTextureCoordinate6, 3.5)
 		+ texture2D(ScreenTexture, FragmentTextureCoordinate7, 4.5));
}
