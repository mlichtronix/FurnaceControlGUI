/*
	Name:    FurnaceController.ino
	Created: 9/13/2019 2:22:25 PM
	Author:  Mlichtronix
*/

// Includes -----------------------
#include <SPI.h>
#include <Wire.h>
#include "DS1302.h"
#include "Classes.h"
#include <LinkedList.h>
#include <Adafruit_MCP9600.h>
#include <Adafruit_I2CDevice.h>
#include <Adafruit_I2CRegister.h>

// Definitions ---------------------
#define BAUDS			9600	// Serial port Baud Rate
#define TRESHOLD		5		// Unde-rtemeperature limit
#define DATESEPARATOR	'-'		// DateTime Separating Character
#define WATTAGEDELAYMAX	5000	// Maximum heating delay cycles
#define SMOKESTACKTEMP	400		// Temperature when it is necessary to close smokestack

// Message Type Codes --------------
#define NoOp				100
#define HandShake			200
#define SetTime				300
#define GetCurTemperature	400
#define GetPcSatus			500
#define GetCurrentProgram	600
#define SetProgram			650
#define Start				700
#define CloseSmokeAlert		800
#define LogMessage			900
#define Heating				950
#define Error				990
#define Invalid				995
#define Halt				999

// Pinout settings -------------------------
int pinSpkr		= 11;	// Speaker Pin
int pinPower	= 10;	// Enable Relay Input
int pinSupply	= 9;	// LOW 10kW - HIGH 30kW
int pinHeat		= 8;	// Enable heating
int pinUp		= 7;	// Push Button Up
int pinDown		= 6;	// Push Button Down
int pinLeft		= 5;	// Push Button Left
int pinRight	= 4;	// Push Button Right
int pinOk		= 3;	// Push Button Left

// Internal variables ----------------------
DS1302 rtc;				// Real Time Module
Adafruit_MCP9600 mcp;	// Thermocouple AD module
LinkedList<ProgramBlock> * Program = new LinkedList<ProgramBlock>();	// Currently set Firing Program

// FiringProgram values:
DateTime ScheduleTime;			// Time Schedule of firing
ProgramBlock::Wattage wattage;	// Maximum heating engagement of current node
String ProgramName = "";		// Name of current set firing program
long temperingEnd = 0;			// End of current tempering in seconds
bool tempering = false;			// Target Temperature is reached and furnace is in tempering mode
bool halted = true;				// System is halted
bool smokeStackOpen = true;		// Smokestack status (initially open)
long wattageDelay = 0;			// Delay counter to full power (Power grid utility safety procedure: 10kW--10s->30kW)
int programCounter = -1;		// Id of active program node
int remainingTime = 0;			// Remaining time of tempering period of current node
int currentTemp = 0;			// Current temperature in furnace
int button = 0;					// Pressed button

void SetPlan()
{
	// Return if halted / Before scheduled time / Some remaining time left
	if (halted || rtc.Now().ToSeconds() < ScheduleTime.ToSeconds() || remainingTime > 0) { return; }
	
	// Set Next Block if available
	if (programCounter + 1 < Program->size())
	{
		tempering = false;
		programCounter++;
		SendMessage(GetPcSatus, String(programCounter));
	}
	else
	{
		// Finish firing after last block
		HaltAndReset();
	}
}

// Update remaining time of current tempering
void UpdateRemainingTime()
{
	// Return if halted
	if (halted || programCounter == -1) { return; }

	// Tempering mode?
	if (tempering)
	{
		// Count down remaining time
		remainingTime = temperingEnd - rtc.Now().ToSeconds();
	}
	else
	{
		ProgramBlock block = Program->get(programCounter);

		// Is target temeperature reached?
		if (block.isTargetReached(currentTemp, TRESHOLD))
		{
			// Set tempering values
			tempering = true;							// Set Tempering mode
			remainingTime = block.duration * 60;		// Set Remaining time
			temperingEnd = rtc.Now().ToSeconds() + remainingTime; // Set when is temepring finished
		}
		else
		{
			// Keep resetting remaining time until main goal is reached
			remainingTime = 1;
			temperingEnd = rtc.Now().ToSeconds() + remainingTime;
		}
	}
}

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
	ReadTemperature();
	ReadSerial();
	SetPlan();
	SetRelays();
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
}

// Check if String contains only numeric values
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

// Convert Current Firing Program to String reperesentation
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

// Split string into LinkedList<String> by delimiter character
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
		// Add trailning leftovers
		output.add(data.substring(last));
	}
	return output;
}

// Parse Firing Program from String reperesentation
bool ParseProgram(String data)
{
	// Data example:
	// "Custom program|400*30*10;960*30*30;1200*60*30"

	LinkedList<String> headTail = Split(data, '|');
	String progName = headTail.get(0);

	// Split blocks
	LinkedList<String> blocksStr = Split(headTail.get(1), ';');
	LinkedList<ProgramBlock> *blocks = new LinkedList<ProgramBlock>();

	// Process all blocks
	for (int i = 0; i < blocksStr.size(); i++)
	{
		// Split values
		LinkedList<String> values = Split(blocksStr.get(i), '*');

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
			return false; // Error occured
		}
	}
	Program = blocks;
	ProgramName = progName;
	return true; // Program is parsed correctly
}

// Read communication from Serial port
void ReadSerial()
{
	// Is something on serial port?
	if (Serial.available()) 
	{
		// Read only one line at the time (1 line = 1 command)
		String message = Serial.readStringUntil('\n');
		if (message.length() >= 4)
		{
			// Message format: <MessageTypeCode>:<MessageData>
			// Example: "650:Custom program|400*30*10"

			String typeStr = message.substring(0, 3);	// Separate Message Code
			String dataStr = message.substring(4);		// Separate Message Data
			typeStr.trim();
			dataStr.trim();
			if (isNumber(typeStr))
			{
				Response(typeStr.toInt(), dataStr);
				return;
			}
		}
		SendMessage(Invalid, message);	// Error occured
	}
}

// Send message over Srial Port
void SendMessage(int t, String msg)
{
	if (Serial)
	{
		Serial.println(String(t) + ":" + msg);
	}
}

// response to commands from Serial port
void Response(int t, String p)
{
	switch (t)
	{
	case GetCurTemperature:
		// 400 - Return Current Temperature in Furnace
		SendMessage(t, String(currentTemp));
		return;

	case GetPcSatus:
		// 500 - Return Current Program Block number
		SendMessage(t, String(programCounter));
		return;

	case Start:
		// 700 - Set Schedule time
		ScheduleTime = DateFromString(p);
		programCounter = -1;
		halted = false;
		SendMessage(t, ScheduleTime.ToFurnaceString());
		return;

	case HandShake:
		// 200 - Return ID of this device according official manufacturer documentation
		SendMessage(t, "CEP-0.5-1150");
		return;

	case SetTime:
		// 300 - Set RealTime
		SetRealTime(DateFromString(p));
		SendMessage(t, rtc.Now().ToFurnaceString());
		return;

	case GetCurrentProgram:
		// 600 - Return current program
		SendMessage(t, ProgramToString());
		return;

	case SetProgram:
		// 650 - Set Custom program
		if (ParseProgram(p))
		{
			SendMessage(t, "Program Set");
		}
		else
		{
			SendMessage(Error, "Canot parse Program!");
		}
		return;

	case Halt:
		// 999 - Stop any activity and reset all settings to default position
		HaltAndReset();
		return;
	}
	SendMessage(Invalid, "Unsupported command: [" + String(t) + ":" + p + "]");
}

// Measure Temperature in furnace
void ReadTemperature()
{
	currentTemp = mcp.readThermocouple();

	// Alarm for manually closing smokestack
	if (!halted && currentTemp >= SMOKESTACKTEMP && smokeStackOpen == true)
	{
		smokeStackOpen = false;
		SendMessage(CloseSmokeAlert, "Please close smokestack!");
		PlaySound("SmokeStack.vaw");
	}
}

// Set heating in furnace according to target temperature
void SetHeating(ProgramBlock::Wattage w)
{
	// Avoid unnecessary updating value
	if (w == wattage) { return; }

	wattage = w;		// Update wattage to new value
	switch (wattage)
	{
	case 0:	// Disable heating
		wattageDelay = 0;				// Reset delay
		digitalWrite(pinHeat, LOW);		// Turn off heat
		delay(250);						// Allow relays to flip
		digitalWrite(pinSupply, LOW);	// Set Power Drain to 10kW
		break;

	case 10:	// Enable low heating
		digitalWrite(pinSupply, LOW);	// Set Power Drain to 10kW
		delay(250);						// Allow relays to flip
		digitalWrite(pinHeat, HIGH);	// Turn on heat
		break;

	case 30:	// Enable full heating
		digitalWrite(pinSupply, HIGH);	// Set Power Drain to 30kW
		delay(250);						// Allow relays to flip
		digitalWrite(pinHeat, HIGH);	// Turn on heat
		break;

	default:			// Something went wrong
		SendMessage(Error, "Wrong Wattage value: [" + String(wattage) + "]");
		break;
	}
	// Send Update Message
	SendMessage(Heating, String(wattage));
}

// Set Power Relays via SSR Array Module
void SetRelays()
{
	// Do nothing if halted
	if (halted || programCounter == -1)
	{
		SetHeating(ProgramBlock::Wattage0kW);
		return;
	}

	// Update wattage delay if is under maxvalue
	if (wattageDelay < WATTAGEDELAYMAX)
	{
		wattageDelay++;
	}

	// Select current program block
	ProgramBlock block = Program->get(programCounter);

	// Temperature reached target value?
	if (currentTemp >= block.temp)
	{
		SetHeating(ProgramBlock::Wattage0kW);				// Disable heating
	}
	else
	{
		// Temperature dropped under threshold value?
		if (currentTemp <= block.temp - 10)
		{
			// 10kW only?
			if (block.drain == 10)
			{
				SetHeating(ProgramBlock::Wattage10kW);		// Set Low Power
			}
			else
			{
				// Passed 15s after 10kW was engaged?
				if (wattageDelay >= WATTAGEDELAYMAX)
				{
					SetHeating(ProgramBlock::Wattage30kW);	// Set Full Power
				}
				else
				{
					SetHeating(ProgramBlock::Wattage10kW);	// Set Low Power
				}
			}
		}
	}
}

// Halt system and reset values to defaults
void HaltAndReset()
{
	SetHeating(ProgramBlock::Wattage0kW);
	halted = true;
	tempering = false;
	smokeStackOpen = true;
	programCounter = -1;
	remainingTime = 0;
	wattageDelay = 0;
	SendMessage(Halt, "Halted");
	PlaySound("Halted.vaw");
}

// Read pressed buttons
void ReadKeyboard()
{
	button = 0;
	if (digitalRead(pinUp)) { button = 1; }
	if (digitalRead(pinDown)) { button = 2; }
	if (digitalRead(pinLeft)) { button = 3; }
	if (digitalRead(pinRight)) { button = 4; }
	if (digitalRead(pinOk)) { button = 5; }
	if (button > 0) { delay(250); }
}

// Navigate trough menu on LCD display
void ConsumeKeyboard()
{
	if (button > 0)
	{
		// TODO Implement keyboard actions
		button = 0;
	}
}

// Draw Display on I2C OLED Module
void DisplayValues()
{
	// TODO: Implement Furnace I2C Display
}

// Play selected sound file from SD Card
void PlaySound(String soundName)
{
	// TODO: Implement PlaySound from SD Card
}

// Parse DateTime from string representation
DateTime DateFromString(String s)
{
	LinkedList<String> values = Split(s, DATESEPARATOR);
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
	}

	if (allOk)
	{
		// Initialize DateTime values
		DateTime t;
		t.Year = values.get(0).toInt();
		t.Month = values.get(1).toInt();
		t.Day = values.get(2).toInt();
		t.Hours = values.get(3).toInt();
		t.Minutes = values.get(4).toInt();
		t.Seconds = values.get(5).toInt();
		return t;
	}

	// String is not in cerrect format
	SendMessage(Error, "Canot convert DateTimeString[" + s + "]");
	return DateTime();
}

// Set time in RTC module
void SetRealTime(DateTime t)
{
	rtc.init(t.Seconds, t.Minutes, t.Hours, t.Day, t.Month, t.Year);
}