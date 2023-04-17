#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

uniform vec4 color = vec4(1, 1, 1, 1);

void main()
{
    outputColor = texture(texture0, texCoord) * color;
}