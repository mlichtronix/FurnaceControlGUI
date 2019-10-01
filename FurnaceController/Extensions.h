#pragma once
#include <Arduino.h>
#include "DateTime.h"
#include <LinkedList.h>
#include "ProgramBlock.h"
#include "FiringProgram.h"

// Check if String contains only numeric values
bool isNumber(String data)
{
	int negative = data.length() > 0 && data[0] == '-' ? 1 : 0;
	for (int i = negative; i < data.length(); i++)
	{
		if (!isDigit(data[i]))
		{
			return false;
		}
	}	
	return data.length() > negative;
}

// Split string into LinkedList<String> by delimiter character
LinkedList<String> SplitString(String data, char separator)
{
	LinkedList<String> output = LinkedList<String>();
	int l = data.length();
	int last = 0;
	for (int f = 0; f < l; f++)
	{
		for (int t = f; t < l; t++)
		{
			if (data[t] == separator)
			{
				last = t + 1;
				output.add(data.substring(f, t));
				f = t;
				break;
			}
		}
	}
	if (last < l)
	{
		// Add trailning leftovers
		output.add(data.substring(last));
	}
	return output;
}

// Parse DateTime from string representation
DateTime DateFromString(String s, char separator)
{
	DateTime time;
	time.WasParsingValid = false;
	LinkedList<String> values = SplitString(s, separator);
	bool allOk = values.size() == 6;	// Is Correct count of parts?
	if (allOk)
	{
		// Check if only numeric characters
		for (int i = 0; i < values.size(); i++)
		{
			if (!isNumber(values.get(i)))
			{
				allOk = false;
				break;
			}
		}
		if (allOk)
		{
			// Initialize DateTime values
			time.Year = values.get(0).toInt();
			time.Month = values.get(1).toInt();
			time.Day = values.get(2).toInt();
			time.Hours = values.get(3).toInt();
			time.Minutes = values.get(4).toInt();
			time.Seconds = values.get(5).toInt();
			time.WasParsingValid = true;
		}
	}
	values.clear();
	return time;
}

// Parse Firing Program from String reperesentation
FiringProgram * ParseProgram(String data)
{
	// Data example:
	// "Custom program|400*30*10;960*30*30;1200*60*30"

	FiringProgram * p = new FiringProgram();
	p->WasParsingValid = false;
	LinkedList<String> headTail = SplitString(data, '|');
	p->Name = headTail.get(0);

	// Split blocks
	LinkedList<String> blocksStr = SplitString(headTail.get(1), ';');
	LinkedList<ProgramBlock> * blocks = new LinkedList<ProgramBlock>();
	// Process all blocks
	for (int i = 0; i < blocksStr.size(); i++)
	{
		// Split values
		LinkedList<String> values = SplitString(blocksStr.get(i), '*');
		
		// Verify that all values are valid
		if (values.size() == 3 &&
			isNumber(values.get(0)) &&
			isNumber(values.get(1)) &&
			isNumber(values.get(2)))
		{
			// Add new block to program
			ProgramBlock block;
			block.temp = values.get(0).toInt();
			block.duration = values.get(1).toInt();
			block.drain = ProgramBlock::Wattage(values.get(2).toInt());
			blocks->add(block);
		}
		else
		{
			return p; // Error occured
		}
	}

	// Initialise new values
	p->Blocks = blocks;
	p->WasParsingValid = true;

	// Program is parsed correctly
	return p;
}