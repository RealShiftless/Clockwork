#version 330 core

layout(location = 0) in vec3 aPosition;

layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 transform;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    texCoord = aTexCoord;

    vec4 csPos = vec4(aPosition, 1.0) * transform * view;
    gl_Position = csPos * projection;
}