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

// Converts FiringProgram to String reperesentation
String FiringProgram::ToString()
{
	String nameDate = Name + "|";
	String blocks = "";
	int s = Blocks.size();
	for (int i = 0; i < s; i++)
	{
		ProgramBlock block = Blocks.get(i);
		blocks += String(block.temp) + "*" + String(block.duration) + "*" + String(block.drain);
		if (i + 1 < s)
		{
			blocks += ";";
		}
	}
	return nameDate + blocks;
}

int FiringProgram::Size() 
{
	return Blocks.size();
}

ProgramBlock FiringProgram::get(int index)
{
	return Blocks.get(index);
}