/*
	Name:    FurnaceController.ino
	Created: 9/13/2019 2:22:25 PM
	Author:  Mlichtronix
*/

// Includes -----------------------
#pragma once
#include "Definitions.h";
#include <SPI.h>
#include <Wire.h>
#include "DS1302.h"
#include <Arduino.h>
#include "DateTime.h"
#include <LinkedList.h>
#include "ProgramBlock.h"
#include "FiringProgram.h"
#include <Adafruit_MCP9600.h>
#include <Adafruit_I2CDevice.h>
#include <Adafruit_I2CRegister.h>

// Internal variables ----------------------
DS1302 rtc;					// Real Time Module
Adafruit_MCP9600 tcm;		// Thermocouple AD module
FiringProgram * Program;	// Currently set Firing Program

#include "Button.h"
#include "Extensions.h"
#include "PredefinedPrograms.h"

// FiringProgram values:
DateTime ScheduleTime;			// Time Schedule of firing
ProgramBlock::Wattage wattage;	// Maximum heating engagement of current node
long temperingEnd = 0;			// End of current tempering in seconds
bool enableAudio = true;		// Audio Signalization Enbled
bool tempering = false;			// Target Temperature is reached and furnace is in tempering mode
bool halted = true;				// System is halted
bool smokeStackOpen = true;		// Smokestack status (initially open)
long wattageDelay = 0;			// Delay counter to full power (Power grid utility safety procedure: 10kW--10s->30kW)
int programCounter = -1;		// Id of active program node
int remainingTime = 0;			// Remaining time of tempering period of current node
int currentTemp = 0;			// Current temperature in furnace

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
	if (!tcm.begin())
	{
		SendMessage(Error, "Sensor MCP9600 not found!");
		while (1);
	}
	tcm.setADCresolution(MCP9600_ADCRESOLUTION_18);
	tcm.setThermocoupleType(MCP9600_TYPE_K);
	tcm.setFilterCoefficient(3);
	tcm.enable(true);

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
	if (Serial && Serial.availableForWrite())
	{
		Serial.println(String(t) + ":" + msg);
	}
}

// response to commands from Serial port
// @param t - MessageTypeCode
// @param p - Additional Parameters
void Response(int t, String p)
{
	switch (t)
	{
		case HandShake:
		{
			// 200 - Return ID of this device according official manufacturer documentation
			SendMessage(t, "CEP-0.5-1150");
			return;
		}
		case SetTime:
		{
			// 300 - Set RealTime
			DateTime realtime = DateFromString(p, DATESEPARATOR);
			if (realtime.WasParsingValid)
			{
				SetRealTime(realtime);
				SendMessage(t, rtc.Now().ToFurnaceString());
			}
			else
			{
				SendMessage(Error, "Canot convert DateTime from String[" + p + "]");
			}
			return;
		}
		case GetCurTemperature:
		{
			// 400 - Return Current Temperature in Furnace
			SendMessage(t, String(currentTemp));
			return;
		}
		case GetPcSatus:
		{
			// 500 - Return Current Program Block number
			SendMessage(t, String(programCounter));
			return;
		}
		case GetCurrentProgram:
		{
			// 600 - Return current program
			SendMessage(t, Program->ToString());
			return;
		}
		case SetProgram:
		{
			SetProgramFromString(p);
			return;
		}
		case Start:
		{
			// 700 - Set Schedule time
			DateTime schedule = DateFromString(p, DATESEPARATOR);
			if (schedule.WasParsingValid)
			{
				halted = false;
				programCounter = -1;
				ScheduleTime = schedule;
				SendMessage(t, ScheduleTime.ToFurnaceString());
			}
			else
			{
				SendMessage(Error, "Canot convert DateTime from String[" + p + "]");
			}
			return;
		}
		case Halt:
		{
			// 999 - Stop any activity and reset all settings to default position
			HaltAndReset();
			return;
		}
		default:
		{
			SendMessage(Invalid, "Unsupported command: [" + String(t) + ":" + p + "]");
			break;
		}
	}
}

// Manual selection using builtin keys
void SetPredefinedProgram(int index) 
{
	SetProgramFromString(predefined[index]);
}

void SetProgramFromString(String p)
{
	// 650 - Set Custom program
	FiringProgram * tmp = ParseProgram(p);
	if (tmp->WasParsingValid)
	{
		Program = tmp;
		SendMessage(SetProgram, "Program set to: [" + Program->Name + "]");
	}
	else
	{
		SendMessage(Error, "Cannot parse Program! [" + p + "]");
	}
}

// Measure Temperature in furnace
void ReadTemperature()
{	
	currentTemp = tcm.readThermocouple();

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

	// Select current program block
	ProgramBlock block = Program->get(programCounter);

	// Update wattage delay for 30kW if is under maxvalue
	if (block.drain == ProgramBlock::Wattage30kW && wattageDelay < WATTAGEDELAYMAX)
	{
		wattageDelay++;
	}

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

// Draw Display on I2C OLED Module
void DisplayValues()
{
	// TODO: Implement Furnace I2C Display
}

// Play selected sound file from SD Card
void PlaySound(String soundName)
{
	if (enableAudio) 
	{
		// TODO: Implement PlaySound from SD Card
	}
}

// Set time in RTC module
void SetRealTime(DateTime t)
{
	rtc.init(t.Seconds, t.Minutes, t.Hours, t.Day, t.Month, t.Year);
}

#include "Automaton.h"
Automaton menu;	// Manual Menu handler

// Navigate trough menu on LCD display
void ReadKeyboard()
{
	Button pressed = ButtonNone;
	if (digitalRead(pinUp)) { pressed = ButtonPlus; }
	if (digitalRead(pinDown)) { pressed = ButtonMinus; }
	if (digitalRead(pinLeft)) { pressed = ButtonLeft; }
	if (digitalRead(pinRight)) { pressed = ButtonRight; }
	if (digitalRead(pinOk)) { pressed = ButtonOk; }
	if (pressed != ButtonNone)
	{
		SendMessage(LogMessage, "Pressed: [" + String(pressed) + "]" );
		menu.OnKeyPress(pressed);
		delay(250);
	}
}