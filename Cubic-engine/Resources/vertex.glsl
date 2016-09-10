#version 430 core				

uniform mat4 camera;
uniform vec3 lightDirection;
uniform vec4 lightColor;

in vec3 position;
in vec3 normal;
in vec2 uv;
in vec3 instancePosition;
in vec3 instanceMaterialDirection;
in int instanceMaterialOffset;
in int instanceMaterialCount;

out vec3 internalPosition;
out vec3 materialDirection;
flat out int materialOffset;
flat out int materialCount;
out float lambert;
out vec2 uvPos;

void main() 
{
	internalPosition = position;
	materialDirection = normalize(instanceMaterialDirection);
	materialOffset = instanceMaterialOffset;
	materialCount = instanceMaterialCount;

	uvPos = uv;

	lambert = max(0, dot(normalize(normal), lightDirection));

	vec3 pos = position;
	pos += instancePosition;

	gl_Position = camera * vec4(pos, 1.0);
}