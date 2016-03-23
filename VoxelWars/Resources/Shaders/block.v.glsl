uniform mat4 mvp;
// uniform float timer;

attribute vec2 coord;
uniform vec3 color;

varying vec3 frag_color;

void main(void) {
	frag_color = color;
	gl_Position = mvp * vec4(coord, 0, 1);
}
