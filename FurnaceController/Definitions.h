#pragma once

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