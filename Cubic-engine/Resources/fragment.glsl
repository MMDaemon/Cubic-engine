#version 430 core

out vec4 color;

flat in vec4 lighting;

void main() 
{
	color = lighting;
}