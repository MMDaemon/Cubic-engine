#version 430 core				

uniform mat4 camera;
uniform vec3 lightDirection;
uniform vec4 lightColor;
uniform int materialCount;

in vec3 position;
in vec3 normal;
in vec2 uv;
in vec3 instancePosition;
in int instanceMaterial;
in vec3 instanceMaterialDirection;

out vec3 internalPosition;
out vec3 materialDirection;
out float materialDirectionLength;
out float lambert;
out vec2 uvPos;

/*float CalculateLengthForDirection(in vec3 materialDirection)
{
	float length = 0.0f;
	if(materialDirection.x > materialDirection.y)
	{
		if(materialDirection.x > materialDirection.z)
		{
			length = dot(vec3(1, 0, 0), normalize(materialDirection));
		}
		else
		{
			length = dot(vec3(0, 0, 1), normalize(materialDirection));
		}
	}
	else
	{
		if(materialDirection.y>materialDirection.z)
		{
			length = dot(vec3(0, 1, 0), normalize(materialDirection));
		}
		else
		{
			length = dot(vec3(0, 0, 1), normalize(materialDirection));
		}
	}
	return length;
}*/

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
	return dot(maxDistVec, normalize(materialDirection));
}

void main() 
{
	internalPosition = position;
	materialDirection = normalize(instanceMaterialDirection);
	materialDirectionLength = CalculateLengthForDirection(instanceMaterialDirection);

	uvPos = vec2(((0.1 + 0.8 * uv.x) + instanceMaterial) / materialCount, 0.1 + 0.8 * uv.y);

	lambert = max(0, dot(normalize(normal), lightDirection));

	vec3 pos = position;
	pos += instancePosition;

	gl_Position = camera * vec4(pos, 1.0);
}