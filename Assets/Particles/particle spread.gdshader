// NOTE: Shader automatically converted from Godot Engine 4.1.1.stable.mono's ParticleProcessMaterial.

shader_type particles;
uniform vec3 direction;
uniform float spread;
uniform float flatness;
uniform float initial_linear_velocity_min;
uniform float initial_angle_min;
uniform float angular_velocity_min;
uniform float orbit_velocity_min;
uniform float linear_accel_min;
uniform float radial_accel_min;
uniform float tangent_accel_min;
uniform float damping_min;
uniform float scale_min;
uniform float hue_variation_min;
uniform float anim_speed_min;
uniform float anim_offset_min;
uniform float initial_linear_velocity_max;
uniform float initial_angle_max;
uniform float angular_velocity_max;
uniform float orbit_velocity_max;
uniform float linear_accel_max;
uniform float radial_accel_max;
uniform float tangent_accel_max;
uniform float damping_max;
uniform float scale_max;
uniform float hue_variation_max;
uniform float anim_speed_max;
uniform float anim_offset_max;
uniform float lifetime_randomness;
uniform vec3 emission_ring_axis;
uniform float emission_ring_height;
uniform float emission_ring_radius;
uniform float emission_ring_inner_radius;
uniform float angle_step;
uniform vec4 color_value : source_color;
uniform vec3 gravity;
uniform sampler2D color_initial_ramp : repeat_disable;


float rand_from_seed(inout uint seed) {
	int k;
	int s = int(seed);
	if (s == 0)
	s = 305420679;
	k = s / 127773;
	s = 16807 * (s - k * 127773) - 2836 * k;
	if (s < 0)
		s += 2147483647;
	seed = uint(s);
	return float(seed % uint(65536)) / 65535.0;
}

float rand_from_seed_m1_p1(inout uint seed) {
	return rand_from_seed(seed) * 2.0 - 1.0;
}

uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}

void start() {
	uint base_number = NUMBER;
	uint alt_seed = hash(base_number + uint(1) + RANDOM_SEED);
	float angle_rand = rand_from_seed(alt_seed);
	float scale_rand = rand_from_seed(alt_seed);
	float hue_rot_rand = rand_from_seed(alt_seed);
	float anim_offset_rand = rand_from_seed(alt_seed);
	float color_initial_rand = rand_from_seed(alt_seed);
	float pi = 3.14159;
	float degree_to_rad = pi / 180.0;

	float tex_angle = 1.0;
	float tex_anim_offset = 1.0;
	float spread_rad = spread * degree_to_rad;
	if (RESTART_VELOCITY) {
		float angle1_rad = float(INDEX) * angle_step * degree_to_rad;
        vec3 rot = vec3(cos(angle1_rad), sin(angle1_rad), 0.0);
        VELOCITY = rot * initial_linear_velocity_min;
	}
	float base_angle = (tex_angle) * mix(initial_angle_min, initial_angle_max, angle_rand);
	CUSTOM.x = base_angle * degree_to_rad;
	CUSTOM.y = 0.0;
	CUSTOM.w = (1.0 - lifetime_randomness * rand_from_seed(alt_seed));
	CUSTOM.z = (tex_anim_offset) * mix(anim_offset_min, anim_offset_max, anim_offset_rand);

	if (RESTART_ROT_SCALE) {
		TRANSFORM[0].xyz = vec3(1.0, 0.0, 0.0);
		TRANSFORM[1].xyz = vec3(0.0, 1.0, 0.0);
		TRANSFORM[2].xyz = vec3(0.0, 0.0, 1.0);
	}

	if (RESTART_POSITION) {
		float ring_spawn_angle = rand_from_seed(alt_seed) * 2.0 * pi;
		float ring_random_radius = rand_from_seed(alt_seed) * (emission_ring_radius - emission_ring_inner_radius) + emission_ring_inner_radius;
		vec3 axis = normalize(emission_ring_axis);
		vec3 ortho_axis = vec3(0.0);
		if (axis == vec3(1.0, 0.0, 0.0)) {
			ortho_axis = cross(axis, vec3(0.0, 1.0, 0.0));
		} else {
 			ortho_axis = cross(axis, vec3(1.0, 0.0, 0.0));
		}
		ortho_axis = normalize(ortho_axis);
		float s = sin(ring_spawn_angle);
		float c = cos(ring_spawn_angle);
		float oc = 1.0 - c;
		ortho_axis = mat3(
			vec3(c + axis.x * axis.x * oc, axis.x * axis.y * oc - axis.z * s, axis.x * axis.z *oc + axis.y * s),
			vec3(axis.x * axis.y * oc + s * axis.z, c + axis.y * axis.y * oc, axis.y * axis.z * oc - axis.x * s),
			vec3(axis.z * axis.x * oc - axis.y * s, axis.z * axis.y * oc + axis.x * s, c + axis.z * axis.z * oc)
			) * ortho_axis;
		ortho_axis = normalize(ortho_axis);
		TRANSFORM[3].xyz = ortho_axis * ring_random_radius + (rand_from_seed(alt_seed) * emission_ring_height - emission_ring_height / 2.0) * axis;
	if (RESTART_VELOCITY) VELOCITY = (EMISSION_TRANSFORM * vec4(VELOCITY, 0.0)).xyz;
	TRANSFORM = EMISSION_TRANSFORM * TRANSFORM;
	VELOCITY.z = 0.0;
	TRANSFORM[3].z = 0.0;
	}
}

void process() {
	uint base_number = NUMBER;
	uint alt_seed = hash(base_number + uint(1) + RANDOM_SEED);
	float angle_rand = rand_from_seed(alt_seed);
	float scale_rand = rand_from_seed(alt_seed);
	float hue_rot_rand = rand_from_seed(alt_seed);
	float anim_offset_rand = rand_from_seed(alt_seed);
	float color_initial_rand = rand_from_seed(alt_seed);
	float pi = 3.14159;
	float degree_to_rad = pi / 180.0;

	CUSTOM.y += DELTA / LIFETIME;
	float tv = CUSTOM.y / CUSTOM.w;
	float tex_linear_velocity = 1.0;
	float tex_orbit_velocity = 1.0;
	float tex_angular_velocity = 1.0;
	float tex_linear_accel = 1.0;
	float tex_radial_accel = 1.0;
	float tex_tangent_accel = 1.0;
	float tex_damping = 1.0;
	float tex_angle = 1.0;
	float tex_anim_speed = 1.0;
	float tex_anim_offset = 1.0;
	vec3 force = gravity;
	vec3 pos = TRANSFORM[3].xyz;
	pos.z = 0.0;
	// apply linear acceleration
	force += length(VELOCITY) > 0.0 ? normalize(VELOCITY) * tex_linear_accel * mix(linear_accel_min, linear_accel_max, rand_from_seed(alt_seed)) : vec3(0.0);
	// apply radial acceleration
	vec3 org = EMISSION_TRANSFORM[3].xyz;
	vec3 diff = pos - org;
	force += length(diff) > 0.0 ? normalize(diff) * tex_radial_accel * mix(radial_accel_min, radial_accel_max, rand_from_seed(alt_seed)) : vec3(0.0);
	// apply tangential acceleration;
	float tangent_accel_val = tex_tangent_accel * mix(tangent_accel_min, tangent_accel_max, rand_from_seed(alt_seed));
	force += length(diff.yx) > 0.0 ? vec3(normalize(diff.yx * vec2(-1.0, 1.0)), 0.0) * tangent_accel_val : vec3(0.0);
	force += ATTRACTOR_FORCE;

	// apply attractor forces
	VELOCITY += force * DELTA;
	
	// orbit velocity
	float orbit_amount = tex_orbit_velocity * mix(orbit_velocity_min, orbit_velocity_max, rand_from_seed(alt_seed));
	if (orbit_amount != 0.0) {
	     float ang = orbit_amount * DELTA * pi * 2.0;
	     mat2 rot = mat2(vec2(cos(ang), -sin(ang)), vec2(sin(ang), cos(ang)));
	     TRANSFORM[3].xy -= diff.xy;
	     TRANSFORM[3].xy += rot * diff.xy;
	}
	float dmp = mix(damping_min, damping_max, rand_from_seed(alt_seed));
	if (dmp * tex_damping > 0.0) {
		float v = length(VELOCITY);
		float damp = tex_damping * dmp;
		v -= damp * DELTA;
		if (v < 0.0) {
			VELOCITY = vec3(0.0);
		} else {
			VELOCITY = normalize(VELOCITY) * v;
		}
	}
	float base_angle = (tex_angle) * mix(initial_angle_min, initial_angle_max, angle_rand);
	base_angle += CUSTOM.y * LIFETIME * (tex_angular_velocity) * mix(angular_velocity_min,angular_velocity_max, rand_from_seed(alt_seed));
	CUSTOM.x = base_angle * degree_to_rad;
	CUSTOM.z = (tex_anim_offset) * mix(anim_offset_min, anim_offset_max, rand_from_seed(alt_seed)) + tv * tex_anim_speed * mix(anim_speed_min, anim_speed_max, rand_from_seed(alt_seed));
	vec3 tex_scale = vec3(1.0);
	float tex_hue_variation = 1.0;
	float hue_rot_angle = (tex_hue_variation) * pi * 2.0 * mix(hue_variation_min, hue_variation_max, rand_from_seed(alt_seed));
	float hue_rot_c = cos(hue_rot_angle);
	float hue_rot_s = sin(hue_rot_angle);
	mat4 hue_rot_mat = mat4(vec4(0.299, 0.587, 0.114, 0.0),
			vec4(0.299, 0.587, 0.114, 0.0),
			vec4(0.299, 0.587, 0.114, 0.0),
			vec4(0.000, 0.000, 0.000, 1.0)) +
		mat4(vec4(0.701, -0.587, -0.114, 0.0),
			vec4(-0.299, 0.413, -0.114, 0.0),
			vec4(-0.300, -0.588, 0.886, 0.0),
			vec4(0.000, 0.000, 0.000, 0.0)) * hue_rot_c +
		mat4(vec4(0.168, 0.330, -0.497, 0.0),
			vec4(-0.328, 0.035,  0.292, 0.0),
			vec4(1.250, -1.050, -0.203, 0.0),
			vec4(0.000, 0.000, 0.000, 0.0)) * hue_rot_s;
	COLOR = hue_rot_mat * color_value;
	vec4 start_color = textureLod(color_initial_ramp, vec2(color_initial_rand, 0.0), 0.0);
	COLOR *= start_color;

	TRANSFORM[0] = vec4(cos(CUSTOM.x), -sin(CUSTOM.x), 0.0, 0.0);
	TRANSFORM[1] = vec4(sin(CUSTOM.x), cos(CUSTOM.x), 0.0, 0.0);
	TRANSFORM[2] = vec4(0.0, 0.0, 1.0, 0.0);
	TRANSFORM[3].z = 0.0;
	float base_scale = mix(scale_min, scale_max, scale_rand);
	base_scale = sign(base_scale) * max(abs(base_scale), 0.001);
	TRANSFORM[0].xyz *= base_scale * sign(tex_scale.r) * max(abs(tex_scale.r), 0.001);
	TRANSFORM[1].xyz *= base_scale * sign(tex_scale.g) * max(abs(tex_scale.g), 0.001);
	TRANSFORM[2].xyz *= base_scale * sign(tex_scale.b) * max(abs(tex_scale.b), 0.001);
	if (CUSTOM.y > CUSTOM.w) {
		ACTIVE = false;
	}
}

