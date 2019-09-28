#pragma once
#include <WString.h>

class DateTime
{
public:
	// Year part of DateTime
	int Year = 0;

	// Month part of DateTime
	int Month = 0;

	// Day in month part of DateTime
	int Day = 0;

	// Hours part of DateTime
	int Hours = 0;

	// Minutes part of DateTime
	int Minutes = 0;

	// Seconds part of DateTime
	int Seconds = 0;

	// Convert DateTime to Furnace String representation
	String ToFurnaceString(void);

	// Determines if parsing from String was valid
	bool WasParsingValid;

	// Convert DateTime to Seconds
	long ToSeconds(void);

	DateTime();
	~DateTime();
};