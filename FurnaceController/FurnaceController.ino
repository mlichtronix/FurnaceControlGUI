/*
 Name:		FurnaceController.ino
 Created:	9/13/2019 2:22:25 PM
 Author:	Mlichtronix
*/

// Includes -----------------------
#include <SPI.h>
#include <Wire.h>
#include "DS1302.h"
#include <LinkedList.h>
#include <Adafruit_MCP9600.h>
#include <Adafruit_I2CDevice.h>
#include <Adafruit_I2CRegister.h>

// Definitions ---------------------
#define I2C_ADDRESS (0x67)      // Adress for MCP9600 Thermocouple Breakout module
#define BAUDS 9600
#define TRESHOLD 5

// Message Type Codes --------------
const int NoOp				= 100;
const int HandShake			= 200;
const int SetTime			= 300;
const int GetCurTemperature = 400;
const int GetPcSatus		= 500;
const int GetCurrentProgram = 600;
const int SetProgram		= 650;
const int Start				= 700;
const int CloseSmokeAlert	= 800;
const int LogMessage		= 900;
const int Heating			= 950;
const int Error				= 990;
const int Invalid			= 995;
const int Halt				= 999;

//  Pinout settings -------------------------
int pinSpkr		= 11;  // Speaker Pin
int pinPower	= 10;  // Enable Relay Input
int pinSupply	= 9;   // LOW 10kW - HIGH 30kW
int pinHeat		= 8;   // Enable heating
int pinUp		= 7;   // Push Button Up
int pinDown		= 6;   // Push Button Down
int pinLeft		= 5;   // Push Button Left
int pinRight	= 4;   // Push Button Right
int pinOk		= 3;   // Push Button Left

//  Internal variables ----------------------
DS1302 rtc;
Adafruit_MCP9600 mcp;
Adafruit_I2CDevice i2c_dev = Adafruit_I2CDevice(I2C_ADDRESS);

bool halted = true;           // System is halted
bool smokeStackOpen = true;   // Smokestack is initially open
int programCounter = -1;      // Id of active program node
int wattageDelay = 0;         // Delay counter to full power;
int wattageDelayMax = 150000; // Maximum delay cycles
int remainingTime = 0;        // Remaining time of current tempering node
int currentTemp = 0;          // Current temperature in furnace
int wattage = 0;              // Heating Wattage
int button = 0;               // Pressed button

class ProgramBlock
{
public:
	int temp = 0;
	int duration = 0;
	int drain = 10;
	bool isTargetReached(int t);
	ProgramBlock();
	~ProgramBlock();
};

ProgramBlock::ProgramBlock() 
{
}

ProgramBlock::~ProgramBlock() 
{
	~temp;
	~duration;
	~drain;
}

bool ProgramBlock::isTargetReached(int t)
{
	return (t >= temp - TRESHOLD && t <= temp);
}

class FiringProgram
{
public:
	String Name;
	LinkedList<ProgramBlock> Blocks;
};

// FiringProgram values:
String ScheduleTime = "";
String ProgramName = "";
LinkedList<ProgramBlock> * Program = new LinkedList<ProgramBlock>();

void SetPlan()
{
	// TODO: Implement Time Comparison
	// Wait until schedule time is reached
	// if (!halted && Time.GetCurrentTime.toLong() < ScheduleTime.toLong()) { return; }
	
	// Start first block if furnace is halted
	if (programCounter == -1) 
	{
		programCounter = 0;
	}

	// Wait until the main goal is reached
	if (!Program->get(programCounter).isTargetReached(currentTemp)) { return; }

	// Continue after finishing tempering
	if (!halted && remainingTime <= 0)
	{
		// Go to next block
		programCounter++;
		if (programCounter < Program->size())
		{
			ProgramBlock block = Program->get(programCounter);

			// Update values
			remainingTime = block.duration * 60;
		}
		else
		{
			HaltAndReset();
		}
	}
}

void UpdateRemainingTime()
{
	rtc.ToFurnaceString();
	remainingTime--;	// TODO: Rewrite
}

//  Main Program -------------------------------
void setup()
{
	// Hard-Wired Pins
	SetPins();

	// Serial Line	
	Serial.begin(BAUDS);

	// MCP9600 A/D Thermal Sensor
	if (!mcp.begin())
	{
		SendMessage(Error, "Sensor MCP9600 not found!");
		while (1);
	}
	mcp.setADCresolution(MCP9600_ADCRESOLUTION_18);
	mcp.setThermocoupleType(MCP9600_TYPE_K);
	mcp.setFilterCoefficient(3);
	mcp.enable(true);

	// TODO ================================
	// Display
	// SD Card
	// Audio
	// AC Voltage Sensor
}

void loop()
{
	SetPlan();
	ReadTemperature();
	SetRelays();
	ReadSerial();
	ReadKeyboard();
	ConsumeKeyboard();
	DisplayValues();
	UpdateRemainingTime();
}

void SetPins()
{
	// Keyboard Pins
	pinMode(pinUp, INPUT);
	pinMode(pinDown, INPUT);
	pinMode(pinLeft, INPUT);
	pinMode(pinRight, INPUT);
	pinMode(pinOk, INPUT);

	// Led Pin
	pinMode(LED_BUILTIN, OUTPUT);
}

bool isNumber(String data)
{
	for (int i = 0; i < data.length(); i++)
	{
		if (!isDigit(data[i]))
		{
			return false;
		}
	}
	return true;
}

String DateToString(String t)
{
	// TODO Implement DateTime class
	return t;
}

String ProgramToString()
{
	String nameDate = ProgramName + "|";
	String blocks = "";
	int s = Program->size();
	for (int i = 0; i < s; i++)
	{
		ProgramBlock block = Program->get(i);
		blocks += String(block.temp) + "*" + String(block.duration) + "*" + String(block.drain);
		if (i + 1 < s)
		{
			blocks += ";";
		}
	}
	return nameDate + blocks;
}

LinkedList<String> Split(String data, char delimiter)
{
	LinkedList<String> output = LinkedList<String>();
	int l = data.length();
	int last = 0;
	for (int f = 0; f < l; f++)
	{
		for (int t = f; t < l; t++)
		{
			if (data[t] == delimiter)
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
		output.add(data.substring(last));
	}
	return output;
}

bool ParseProgram(String data)
{
	//Data example: "Custom program|400*30*10;960*30*30;1200*60*30"

	LinkedList<String> parts = Split(data, '|');
	String progName = parts.get(0);
	LinkedList<String> blocksStr = Split(parts.get(1), ';');
	LinkedList<ProgramBlock> *blocks = new LinkedList<ProgramBlock>();
	
	for (int i = 0; i < blocksStr.size(); i++)
	{
		LinkedList<String> values = Split(blocksStr.get(i), '*');

		if (values.size() == 3 &&
			isNumber(values.get(0)) &&
			isNumber(values.get(1)) &&
			isNumber(values.get(2))) 
		{
			ProgramBlock block;
			block.temp = values.get(0).toInt();
			block.duration = values.get(1).toInt();
			block.drain = values.get(2).toInt();
			blocks->add(block);
		}
		else
		{
			return false;
		}
	}
	Program = blocks;
	ProgramName = progName;
	return true;
}

void ReadSerial()
{
	if (Serial.available())
	{
		String message = Serial.readStringUntil('\n');
		if (message.length() >= 4)
		{
			String typeStr = message.substring(0, 3);			
			String dataStr = message.substring(4);
			typeStr.trim();
			dataStr.trim();
			if (isNumber(typeStr))
			{
				Response(typeStr.toInt(), dataStr);
				return;
			}
		}
		SendMessage(Invalid, message);
	}
}

void SendMessage(int t, String msg)
{
	if (Serial)
	{
		Serial.println(String(t) + ":" + msg);
	}
}

void Response(int t, String p)
{
	switch (t)
	{
	case HandShake: // 200
		// Return ID of this device according official manufacturer documentation
		SendMessage(t, "CEP-0.5-1150");
		return;

	case Start: // 700
		halted = false;
		programCounter = -1;
		ScheduleTime = DateFromString(p);
		SendMessage(t, DateToString(ScheduleTime));
		return;

	case SetTime: // 300
		SetRealTime(p);
		SendMessage(t, rtc.ToFurnaceString());
		return;

	case GetCurTemperature: // 400
		// Return Current Temperature in Furnace
		SendMessage(t, String(currentTemp));
		return;

	case GetPcSatus:  // 500
		// Return Current Program Block number
		SendMessage(t, String(programCounter));
		return;

	case GetCurrentProgram: // 600
		SendMessage(t, ProgramToString());
		return;

	case SetProgram:  // 650
		if (ParseProgram(p))
		{
			SendMessage(t, "Program Set");
		}
		else
		{
			SendMessage(Error, "Canot parse Program!");
		}
		return;

	case Halt:  // 999
		// Stop any activity and reset all settings to default position
		HaltAndReset();
		return;
	}
	SendMessage(Invalid, "Unsupported command! [" + String(t) + ":" + p + "]");
}

void HaltAndReset()
{
	SetHeating(0);
	halted = true;
	smokeStackOpen = true;
	programCounter = -1;
	remainingTime = 0;
	wattageDelay = 0;
	wattage = 0;
	SendMessage(Halt, "Halted");
}

void ReadTemperature()
{
	currentTemp = mcp.readThermocouple();
	if (currentTemp >= 400 && smokeStackOpen == true)
	{
		// Alarm for manually closing smokestack
		smokeStackOpen = false;
		SendMessage(CloseSmokeAlert, "Please close smokestack!");
		PlaySound("SmokeStack.vaw");
	}
}

void SetHeating(int w)
{
	if (w == wattage)
	{
		return;
	}
	wattage = w;  // Update wattage
	switch (wattage)
	{
	case 0:
		wattageDelay = 0;  // Reset delay
		digitalWrite(pinHeat, LOW);   // Turn off heat
		delay(250);                   // Allow relays to flip
		digitalWrite(pinSupply, LOW); // Set Power Drain to 10kW
		SendMessage(Heating, "0");  // Send Update Message
		break;

	case 10:
		digitalWrite(pinSupply, LOW);  // Set Power Drain to 10kW
		delay(250);                    // Allow relays to flip
		digitalWrite(pinHeat, HIGH);   // Turn on heat
		SendMessage(Heating, "10");  // Send Update Message
		break;

	case 30:
		digitalWrite(pinSupply, HIGH); // Set Power Drain to 30kW
		delay(250);                    // Allow relays to flip
		digitalWrite(pinHeat, HIGH);   // Turn on heat
		SendMessage(Heating, "30");  // Send Update Message
		break;

	default:
		SendMessage(Error, "Wrong Wattage value: [" + String(wattage) + "]");
		break;
	}
}

void SetRelays()
{
	if (halted)
	{
		SetHeating(0);
		return;
	}

	if (wattageDelay < wattageDelayMax)
	{
		wattageDelay++;
	}
	
	ProgramBlock block = Program->get(programCounter);
	// Temperature reached target value
	if (currentTemp >= block.temp)
	{
		SetHeating(0);
	}
	else
	{
		// Temperature dropped under threshold value
		if (currentTemp <= block.temp - 10)
		{
			if (block.drain == 10)
			{
				SetHeating(10);
			}
			else
			{
				if (wattageDelay >= wattageDelayMax)
				{
					SetHeating(30);
				}
				else
				{
					SetHeating(10);
				}
			}
		}
	}
}

void ReadKeyboard()
{
	button = 0;
	if (digitalRead(pinUp)) {
		button = 1;
	}
	if (digitalRead(pinDown)) {
		button = 2;
	}
	if (digitalRead(pinLeft)) {
		button = 3;
	}
	if (digitalRead(pinRight)) {
		button = 4;
	}
	if (digitalRead(pinOk)) {
		button = 5;
	}
	if (button > 0) {
		delay(250);
	}
}

void ConsumeKeyboard()
{
	if (button > 0)
	{
		// TODO Implement keyboard actions
		button = 0;
	}
}

void DisplayValues()
{

}

void PlaySound(String soundName)
{

}

void SetRealTime(String s)
{
	LinkedList<String> values = Split(s, '-');
	bool allOk = true;
	if (values.size() == 6) 
	{
		for (int i = 0; i < values.size(); i++)
		{
			if (!isNumber(values.get(i))) 
			{
				allOk = false;
				break;
			}
		}
		int yrs	= values.get(0).toInt();
		int mon	= values.get(1).toInt();
		int day	= values.get(2).toInt();
		int hrs	= values.get(3).toInt();
		int min	= values.get(4).toInt();
		int sec	= values.get(5).toInt();
		rtc.init(sec, min, hrs, day, mon, yrs);
	}
	if(!allOk)
	{
		SendMessage(Error, "Canot convert DateTimeString[" + s + "]");
	}
}

