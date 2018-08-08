#ifndef IncludedRandom
#define IncludedRandom

#define UIntMax 4294967295.0 // 0xffffffff
#define UIntMap (1.0 / UIntMax)

uint rand_state;

uint rand_hash(uint seed)
{
	seed = (seed ^ 61) ^ (seed >> 16);
	seed *= 9;

	seed = seed ^ (seed >> 4);
	seed *= 0x27d4eb2d;

	seed = seed ^ (seed >> 15);
	return seed;
}

uint rand(inout uint state)
{
	// xorshift.
	state ^= (state << 13);
	state ^= (state >> 17);
	state ^= (state << 5);

	return state;
}

float rand01(inout uint state)
{
	return rand(state) * UIntMap;
}

float rand(inout uint state, float max)
{
	return rand01(state) * max;
}

uint rand()
{
	return rand(rand_state);
}

float rand01()
{
	return rand01(rand_state);
}

float rand01(float max)
{
	return rand(rand_state, max);
}
#endif
