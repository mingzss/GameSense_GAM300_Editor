#version 460

layout (location = 0) in vec2 inUV;

layout (std140, push_constant) uniform PushConstants 
{
	vec4 world_eye_pos;
	vec4 ambient_color;
    vec2 viewport_size;
    int user_param;
    int user_param2;
	int user_param3;
	int user_param4;
    int user_param5;
    int user_param6;
    mat4 shadow_matrix;
} pushConsts;

layout (binding = 5) uniform sampler2D uFinalTexture;

layout (location = 0) out vec4 outFragColor;

void main() 
{
	outFragColor = texture(uFinalTexture, inUV);
}