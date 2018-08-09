#ifndef GEOMETRIES_COMMON_INCLUDED
#define GEOMETRIES_COMMON_INCLUDED

float GetOffset(uint randSeed, float base, float max)
{
	float width = max - base;
	float div = width / 8.0;

	return trunc(rand(randSeed, div)) / max;
}

#endif
