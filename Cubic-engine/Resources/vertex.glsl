#version 430 core				

uniform mat4 camera;
uniform vec3 lightDirection;
uniform vec4 lightColor;


in vec3 position;
in vec3 normal;
in vec3 instancePosition;
in vec4 materialColor;

out vec4 lighting;

void main() 
{
	float lambert = max(0, dot(normalize(normal), lightDirection));
	vec4 diffuse = materialColor * lightColor * lambert;
	lighting = vec4(0.1, 0.1, 0.1, 1) * materialColor * lightColor + diffuse;

	vec3 pos = position;
	pos += instancePosition;

	gl_Position = camera * vec4(pos, 1.0);
}