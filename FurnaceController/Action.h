#pragma once
enum Action
{
	ActionNone = 0,						// No Action required
	ActionStart, ActionHalt,			// Start / Halt furnace	
	ActionSetProgram, ActionProgramI, ActionProgramD,
	ActionTimeYearI, ActionTimeMonthI, ActionTimeDayI, ActionTimeHoursI, ActionTimeMinutesI, ActionTimeSecondsI,		// Set real time - Increase
	ActionTimeYearD, ActionTimeMonthD, ActionTimeDayD, ActionTimeHoursD, ActionTimeMinutesD, ActionTimeSecondsD,		// Set real time - Decrease
	ActionTimeApply,					// Apply new RealTimeClock Values
	ActionSchNow,						// Set RTC as Start Time
	ActionSchYearI, ActionSchMonthI, ActionSchDayI, ActionSchHoursI, ActionSchMinutesI, ActionSchSecondsI,		// Set Schedule time - Increase
	ActionSchYearD, ActionSchMonthD, ActionSchDayD, ActionSchHoursD, ActionSchMinutesD, ActionSchSecondsD,		// Set Schedule time - Decrease
	ActionSound, ActionFailSignal,		// Enable / Disable audio signalization
};