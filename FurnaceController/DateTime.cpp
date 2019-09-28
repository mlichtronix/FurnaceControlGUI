#include "DateTime.h"

DateTime::DateTime() {};
DateTime::~DateTime() {};

// Convert DateTime to Furnace String representation
String DateTime::ToFurnaceString(void)
{
	return String(Year) + "-" + String(Month) + "-" + String(Day) + "-" + String(Hours) + "-" + String(Minutes) + "-" + String(Seconds);
}

// Convert DateTime to Seconds
long DateTime::ToSeconds(void)
{
	long Y = long(Year * 31556926);
	long M = long(Month * 2629744);
	long D = long(Day * 86400);
	long H = long(Hours * 3600);
	long E = long(Minutes * 60);
	return long(Y + M + D + H + E + Seconds);
}