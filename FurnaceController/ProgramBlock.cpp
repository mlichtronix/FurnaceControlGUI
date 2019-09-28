#include "WString.h"
#include "ProgramBlock.h"
#include "FiringProgram.h"

// Implicit ProgramBlock Constructor for LinkedList needs
ProgramBlock::ProgramBlock() {}

// Implicit ProgramBlock Destructor for LinkedList needs
ProgramBlock::~ProgramBlock() {}

// Check if temperature in furnace is in range of current block target temperature
bool ProgramBlock::isTargetReached(int t, int treshold)
{
	return (t >= temp - treshold && t <= temp);
}

// Converts ProgramBlock to String reperesentation
String ProgramBlock::ToString() 
{
	return String(temp) + "*" + String(duration) + "*" + String(drain);
}