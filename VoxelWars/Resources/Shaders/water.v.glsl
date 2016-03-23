uniform mat4 mvp;
uniform float timer;
uniform vec2 offset;

attribute vec2 coord;
attribute float metadata;
uniform vec3 color;

varying vec3 frag_color;

#define WAVE_HEIGHT 0.2
#define WAVE_SPEED 20.0
#define WAVE_LENGTH 7.0

void main(void) {
/*
	frag_color = color;

	vec2 pos = coord;
	pos.y -= WAVE_HEIGHT;

	float posYbuf = ((pos.x + offset.x) / WAVE_LENGTH + timer * WAVE_SPEED * WAVE_LENGTH);

	pos.y -= sin(posYbuf) *  WAVE_HEIGHT + sin(posYbuf / 7.0) * WAVE_HEIGHT;

	gl_Position = mvp * vec4(pos, 0, 1);
*/

	frag_color = color * (1 - (metadata / 1000));
	gl_Position = mvp * vec4(coord, 0, 1);

}
