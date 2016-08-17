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

out float lambert;
out vec2 uvPos;

void main() 
{
	uvPos = vec2(((0.1+0.8*uv.x)+instanceMaterial)/materialCount,0.1+0.8*uv.y);

	lambert = max(0, dot(normalize(normal), lightDirection));

	vec3 pos = position;
	pos += instancePosition;

	gl_Position = camera * vec4(pos, 1.0);
}