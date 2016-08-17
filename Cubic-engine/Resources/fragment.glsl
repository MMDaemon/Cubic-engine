#version 430 core

uniform vec4 lightColor;
uniform sampler2D materialTexture;

in vec2 uvPos;
in float lambert;

out vec4 color;

void main() 
{
	vec4 materialColor = texture2D(materialTexture, uvPos);
	vec4 diffuse = materialColor * lightColor * vec4(lambert, lambert, lambert, 1);
	color = vec4(0.1, 0.1, 0.1, 1) * materialColor * lightColor + diffuse;
}