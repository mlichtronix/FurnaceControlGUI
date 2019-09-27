#include "Classes.h"

// Check if temperature in furnace is in range of current block target temperature
bool ProgramBlock::isTargetReached(int t, int treshold)
{
	return (t >= temp - treshold && t <= temp);
}

// Implicit ProgramBlock Constructor for LinkedList needs
ProgramBlock::ProgramBlock() {}

// Implicit ProgramBlock Destructor for LinkedList needs
ProgramBlock::~ProgramBlock() {}

// Implicit FiringProgram Constructor for LinkedList needs
FiringProgram::FiringProgram() {}

// Implicit FiringProgram Destructor for LinkedList needs
FiringProgram::~FiringProgram() {}