#ifndef GEOMETRIES_COMMON_INCLUDED
#define GEOMETRIES_COMMON_INCLUDED

uniform int _windowNumberX;
uniform int _windowNumberY;

float GetUvOffset(uint randSeed, float base, float max)
{
	float width = max - base;
	float div = width / 8.0;

	return trunc(rand(randSeed, div)) / max;
}

bool IsMultipleOfTwo(int value)
{
	int i = frac(value * 0.1) * 10;
	return i == 0 || i == 2 || i == 4 || i == 6 || i == 8;
}

#endif
