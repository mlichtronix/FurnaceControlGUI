#pragma once
#include "WString.h"
#include "LinkedList.h"
#include "ProgramBlock.h"

class FiringProgram
{
public:
	FiringProgram();

	~FiringProgram();

	// Program name
	String Name;

	// Program nodes (Temperature, Duration, Wattage)
	LinkedList<ProgramBlock> * Blocks;

	// Determines if parsing from String was valid
	bool WasParsingValid = true;

	// Converts FiringProgram to String reperesentation
	String ToString();

	// Get block by index
	ProgramBlock get(int index);

	// Get blocks count
	int size();
};