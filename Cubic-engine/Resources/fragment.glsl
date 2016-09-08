#version 430 core

uniform vec4 lightColor;
uniform sampler2D materialTexture;
uniform int materialCount;

in vec3 internalPosition;
flat in int material;
in vec3 materialDirection;
in vec2 uvPos;
in float lambert;

out vec4 color;

float CalculateLengthForDirection(in vec3 materialDirection)
{
	vec3 maxDistVec = vec3(0,0,0);
	if(materialDirection.x > 0)
	{
		maxDistVec.x = 0.5;
	}
	else
	{
		maxDistVec.x = -0.5;
	}
	if(materialDirection.y > 0)
	{
		maxDistVec.y = 0.5;
	}
	else
	{
		maxDistVec.y = -0.5;
	}
	if(materialDirection.z > 0)
	{
		maxDistVec.z = 0.5;
	}
	else
	{
		maxDistVec.z = -0.5;
	}
	return dot(maxDistVec, materialDirection);
}

void main() 
{
	float materialDirectionLength = CalculateLengthForDirection(materialDirection);
	float materialDirectionHeight = (dot(internalPosition, materialDirection) / materialDirectionLength) + 0.5;
	//color = vec4(materialDirectionHeight, materialDirectionHeight, materialDirectionHeight, 1);

	vec2 uv = vec2((uvPos.x + material) / materialCount, uvPos.y);

	vec4 materialColor = texture2D(materialTexture, uv);
	vec4 diffuse = materialColor * lightColor * vec4(lambert, lambert, lambert, 1);
	color = vec4(0.1, 0.1, 0.1, 1) * materialColor * lightColor + diffuse;
}