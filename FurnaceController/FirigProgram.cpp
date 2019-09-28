#include "FiringProgram.h"

// Implicit FiringProgram Constructor for LinkedList needs
FiringProgram::FiringProgram() 
{
	Name = "Empty Program";
	Blocks = new LinkedList<ProgramBlock>();
}

// Implicit FiringProgram Destructor for LinkedList needs
FiringProgram::~FiringProgram() 
{
	Blocks->clear();
	delete Blocks;
}

// Get count of blocks
int FiringProgram::size()
{
	return Blocks->size();
}

// Get block by index
ProgramBlock FiringProgram::get(int index)
{
	return Blocks->get(index);
}

// Converts FiringProgram to String reperesentation
String FiringProgram::ToString()
{
	String blocks = "";
	int s = Blocks->size();
	for (int i = 0; i < s; i++)
	{
		blocks += Blocks->get(i).ToString() + (i + 1 < s ? ";" : "");
	}
	return Name + "|" + blocks;
}