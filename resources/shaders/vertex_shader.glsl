#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in int aNormal;
layout (location = 2) in int aBlockType;
layout (location = 3) in float aAO;

out vec3 normal;
out vec3 fragPos;
out vec3 color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

const vec3 normals[] = vec3[](
	vec3(0.0, 0.0, -1.0),
	vec3(0.0, 0.0, 1.0),
	vec3(-1.0, 0.0, 0.0),
	vec3(1.0, 0.0, 0.0),
	vec3(0.0, -1.0, 0.0),
	vec3(0.0, 1.0, 0.0)
);

const vec3 colors[] = vec3[](
	vec3(1.0, 1.0, 1.0),
	vec3(0.435, 0.306, 0.216),
	vec3(0.059, 0.369, 0.612)
);

const vec3 ambientOcclusionColor = vec3(0.0f, 0.0f, 0.0f);

void main()
{
	gl_Position = projection * view * model * vec4(aPos, 1.0);
	normal = normals[aNormal];
	color = mix(ambientOcclusionColor, colors[aBlockType], aAO);
	fragPos = vec3(model * vec4(aPos, 1.0f));
}