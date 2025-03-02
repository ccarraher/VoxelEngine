#version 330 core

out vec4 fragColor;

in vec3 normal;
in vec3 fragPos;
in vec3 color;

const vec3 lightColor = vec3(1.0f, 1.0f, 1.0f);
const vec3 lightDir = vec3(-0.2f, -1.0f, -0.3f);

vec3 calculateAmbient()
{
	float ambientStrength = 0.3;
	return ambientStrength * lightColor;	
}

vec3 calculateDiffuse()
{
	vec3 norm = normalize(normal);
	float diff = max(dot(norm, -lightDir), 0.0);
	return diff * lightColor;
}

void main()
{
	vec3 ambient = calculateAmbient();
	vec3 diffuse = calculateDiffuse();
	vec3 result = (ambient + diffuse) * color;
	fragColor = vec4(result, 1.0f);
}