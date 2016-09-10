#version 430 core

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

layout(std430, binding = 3) buffer materialLayout
{
	int materials[];
};

layout(std430, binding = 4) buffer extentLayout
{
	float extents[];
};

uniform vec4 lightColor;
uniform sampler2D materialTexture;
uniform int totalMaterialCount;

in vec3 internalPosition;
in vec3 materialDirection;
flat in int materialOffset;
flat in int materialCount;
in vec2 uvPos;
in float lambert;

out vec4 color;

void main() 
{
	float materialDirectionLength = CalculateLengthForDirection(materialDirection);
	float materialDirectionHeight = (dot(internalPosition, materialDirection) / materialDirectionLength) + 0.5;
	//color = vec4(materialDirectionHeight, materialDirectionHeight, materialDirectionHeight, 1);

	vec2 uv = vec2((uvPos.x + materials[materialOffset]) / totalMaterialCount, uvPos.y);

	float extent=0;
	for(int i = 0; i < materialCount; i++){
		int offset = materialOffset + i;
		if(materialDirectionHeight>=extent){
			uv = vec2((uvPos.x + materials[offset]) / totalMaterialCount, uvPos.y);
		}
		extent += extents[offset];
	}

	vec4 materialColor = texture2D(materialTexture, uv);
	vec4 diffuse = materialColor * lightColor * vec4(lambert, lambert, lambert, 1);
	color = vec4(0.1, 0.1, 0.1, 1) * materialColor * lightColor + diffuse;
}