#version 430 core				

uniform mat4 camera;
uniform vec3 lightDirection;
uniform vec4 lightColor;

in vec3 position;
in vec3 normal;
in vec2 uv;
in vec3 instancePosition;
in int instanceMaterial;
in vec3 instanceMaterialDirection;

out vec3 internalPosition;
flat out int material;
out vec3 materialDirection;
out float lambert;
out vec2 uvPos;

void main() 
{
	internalPosition = position;
	material = instanceMaterial;
	materialDirection = normalize(instanceMaterialDirection);

	uvPos = uv;

	lambert = max(0, dot(normalize(normal), lightDirection));

	vec3 pos = position;
	pos += instancePosition;

	gl_Position = camera * vec4(pos, 1.0);
}