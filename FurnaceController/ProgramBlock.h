#pragma once

class ProgramBlock
{
public:
	// Heating coils engagement configuration
	enum Wattage
	{
		Wattage0kW  =  0,	// Heating Off
		Wattage10kW = 10,	// Coils engagement configuration delta	| A
		Wattage30kW = 30,	// Coils engagement configuration star	| Y
	};

	ProgramBlock();
	~ProgramBlock();

	// Target Temperature
	int temp = 0;

	// Tempering duration
	int duration = 0;

	// Maximum power drain configuration
	Wattage drain = Wattage0kW;

	// Check if temperature in furnace is in range of current block target temperature
	bool isTargetReached(int t, int treshold);

	String ToString();
};
