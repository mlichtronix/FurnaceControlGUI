#pragma once
#include "Button.h"
#include "DateTime.h"
#include "WString.h"
#include "Action.h"
#include "ScreenType.h"
#include "State.h"

class Automaton
{
private:
	State * Current;	// Current Node

	// Node Set
	State nodeMenu; 			// 1
	State nodeFiring;			// 2
	State nodeSettings;			// 3
	State nodeSelectProgram;	// 4
	State nodeProgram;			// 5
	State nodeSchedule;			// 6
	State nodeStart;			// 7
	State nodeHalt;				// 8
	State nodeSchNow;			// 9
	State nodeSchYear;			// 10
	State nodeSchMonth;			// 11
	State nodeSchDay;			// 12
	State nodeSchHours;			// 13
	State nodeSchMinutes;		// 14
	State nodeSchSeconds;		// 15
	State nodeSchApply;			// 16
	State nodeAudio;			// 17
	State nodeTime;				// 18
	State nodeTimeYear;			// 19
	State nodeTimeMonth;		// 20
	State nodeTimeDay;			// 21
	State nodeTimeHours;		// 22
	State nodeTimeMinutes;		// 23
	State nodeTimeSeconds;		// 24
	State nodeTimeApply;		// 25

	void DrawOnDisplay()
	{
		switch (Current->type)
		{
			case ScreenIdle: DrawIdle(); break;
			case ScreenMenu: DrawMenu(); break;
			case ScreenSettings: DrawSettings(); break;
			case ScreenFiring: DrawFiring(); break;
			case ScreenTime: DrawTime(); break;
			case ScreenProgram: DrawPorgram(); break;
			case ScreenNow: DrawNow(); break;
			case ScreenSchedule: DrawSchedule(); break;
			case ScreenHalt: DrawHalt(); break;
			default: DrawError(); break;
		}
	}

	void DrawIdle() {}
	void DrawMenu() {}
	void DrawSettings() {}
	void DrawFiring() {}
	void DrawTime() {}
	void DrawPorgram() {}
	void DrawNow() {}
	void DrawSchedule() {}
	void DrawHalt() {}
	void DrawError() {}

	void DoAction(Action a)
	{
		switch (a)
		{
			// Operation
			case ActionNone: return;									// Do nothing (No key was pressed)
			case ActionFailSignal: PlaySound("FailKey.vaw"); return;	// Play Sound when wronk key pressed
			case ActionStart:	// Set Begin Time and Start
			{
				ScheduleTime.Year = nodeSchYear.GetValue();
				ScheduleTime.Month = nodeSchMonth.GetValue();
				ScheduleTime.Day = nodeSchDay.GetValue();
				ScheduleTime.Hours = nodeSchHours.GetValue();
				ScheduleTime.Minutes = nodeSchMinutes.GetValue();
				ScheduleTime.Seconds = nodeSchSeconds.GetValue();
				halted = false;
				return;
			}
			case ActionHalt: HaltAndReset(); return;					// Halt Furnace and reset
			case ActionSound: enableAudio = !enableAudio; return;		// Enable or Disable Audio Signalization
			case ActionSetProgram: SetPredefinedProgram(Current->GetValue() - 1); return; // Select Predefined Program
			case ActionProgramI: nodeProgram.RotateUp(); return;
			case ActionProgramD: nodeProgram.RotateDown(); return;
				// RTC
			case ActionTimeYearI: nodeTimeYear.RotateUp(); return;
			case ActionTimeYearD: nodeTimeYear.RotateDown(); return;
			case ActionTimeMonthI: nodeTimeMonth.RotateUp(); return;
			case ActionTimeMonthD: nodeTimeMonth.RotateDown(); return;
			case ActionTimeDayI: nodeTimeDay.RotateUp(); return;
			case ActionTimeDayD: nodeTimeDay.RotateDown(); return;
			case ActionTimeHoursI: nodeTimeHours.RotateUp(); return;
			case ActionTimeHoursD: nodeTimeHours.RotateDown(); return;
			case ActionTimeMinutesI: nodeTimeMinutes.RotateUp(); return;
			case ActionTimeMinutesD: nodeTimeMinutes.RotateDown(); return;
			case ActionTimeSecondsI: nodeTimeSeconds.RotateUp(); return;
			case ActionTimeSecondsD: nodeTimeSeconds.RotateDown(); return;
			case ActionTimeApply:
			{
				DateTime tmp;
				tmp.Year = nodeTimeYear.GetValue();
				tmp.Month = nodeTimeMonth.GetValue();
				tmp.Day = nodeTimeDay.GetValue();
				tmp.Hours = nodeTimeHours.GetValue();
				tmp.Minutes = nodeTimeMinutes.GetValue();
				tmp.Seconds = nodeTimeSeconds.GetValue();
				SetRealTime(tmp);
				return;
			}
			// Schedule
			case ActionSchNow:
			{
				DateTime now = rtc.Now();
				nodeSchYear.SetValue(now.Year);
				nodeSchMonth.SetValue(now.Month);
				nodeSchDay.SetValue(now.Day);
				nodeSchHours.SetValue(now.Hours);
				nodeSchMinutes.SetValue(now.Minutes);
				nodeSchSeconds.SetValue(now.Seconds);
				return;
			}
			case ActionSchYearI: nodeSchYear.RotateUp(); return;
			case ActionSchYearD: nodeSchYear.RotateDown(); return;
			case ActionSchMonthI: nodeSchMonth.RotateUp(); return;
			case ActionSchMonthD: nodeSchMonth.RotateDown(); return;
			case ActionSchDayI: nodeSchDay.RotateUp(); return;
			case ActionSchDayD: nodeSchDay.RotateDown(); return;
			case ActionSchHoursI: nodeSchHours.RotateUp(); return;
			case ActionSchHoursD: nodeSchHours.RotateDown(); return;
			case ActionSchMinutesI: nodeSchMinutes.RotateUp(); return;
			case ActionSchMinutesD: nodeSchMinutes.RotateDown(); return;
			case ActionSchSecondsI: nodeSchSeconds.RotateUp(); return;
			case ActionSchSecondsD: nodeSchSeconds.RotateDown(); return;
			default:	// Something went wrong
				SendMessage(990, "Undefined action!");
				break;
		}
	}

public:
	Automaton()
	{
		// Initialise Nodes and Edges
		DateTime now = rtc.Now();

		nodeMenu = State(ScreenIdle);								// 1
		nodeFiring = State(ScreenMenu);								// 2
		nodeSettings = State(ScreenMenu);							// 3
		nodeSelectProgram = State(ScreenFiring);					// 4
		nodeProgram = State(ScreenProgram, 1, 1, 7);				// 5
		nodeSchedule = State(ScreenFiring);							// 6
		nodeStart = State(ScreenFiring);							// 7
		nodeHalt = State(ScreenHalt);								// 8
		nodeSchNow = State(ScreenNow);								// 9
		nodeSchYear = State(ScreenSchedule, now.Year, 2019, 2050);	// 10
		nodeSchMonth = State(ScreenSchedule, now.Month, 1, 12);		// 11
		nodeSchDay = State(ScreenSchedule, now.Day, 1, 31);			// 12
		nodeSchHours = State(ScreenSchedule, now.Hours, 0, 23);		// 13
		nodeSchMinutes = State(ScreenSchedule, now.Minutes, 0, 59);	// 14
		nodeSchSeconds = State(ScreenSchedule, now.Seconds, 0, 59);	// 15
		nodeSchApply = State(ScreenSchedule);						// 16
		nodeAudio = State(ScreenSettings, int(enableAudio), 0, 1);	// 17
		nodeTime = State(ScreenSettings);							// 18
		nodeTimeYear = State(ScreenTime, now.Year, 2019, 2050);		// 19
		nodeTimeMonth = State(ScreenTime, now.Month, 1, 12);		// 20
		nodeTimeDay = State(ScreenTime, now.Day, 1, 31);			// 21
		nodeTimeHours = State(ScreenTime, now.Hours, 0, 23);		// 22
		nodeTimeMinutes = State(ScreenTime, now.Minutes, 0, 59);	// 23
		nodeTimeSeconds = State(ScreenTime, now.Seconds, 0, 59);	// 24
		nodeTimeApply = State(ScreenTime);							// 25

		// Idle Menu
		nodeMenu.label = "Menu";
		nodeMenu.edges.add(Edge(ButtonRight, &nodeFiring));

		// Firing
		nodeFiring.label = "Palenie";
		nodeFiring.edges.add(Edge(ButtonLeft, &nodeMenu));
		nodeFiring.edges.add(Edge(ButtonRight, &nodeSelectProgram));
		nodeFiring.edges.add(Edge(ButtonMinus, &nodeSettings));

		// Settings
		nodeSettings.label = "Nastavenia";
		nodeSettings.edges.add(Edge(ButtonRight, &nodeAudio));
		nodeSettings.edges.add(Edge(ButtonLeft, &nodeMenu));
		nodeSettings.edges.add(Edge(ButtonPlus, &nodeFiring));

		// Select Program
		nodeSelectProgram.label = "Volba programu";
		nodeSelectProgram.edges.add(Edge(ButtonRight, &nodeProgram));
		nodeSelectProgram.edges.add(Edge(ButtonLeft, &nodeFiring));
		nodeSelectProgram.edges.add(Edge(ButtonMinus, &nodeSchedule));

		// Schedule Time
		nodeSchedule.label = "Cas zaciatku";
		nodeSchedule.edges.add(Edge(ButtonRight, &nodeSchNow));
		nodeSchedule.edges.add(Edge(ButtonLeft, &nodeFiring));
		nodeSchedule.edges.add(Edge(ButtonMinus, &nodeStart));
		nodeSchedule.edges.add(Edge(ButtonPlus, &nodeSelectProgram));

		// Start
		nodeStart.label = "Spustit vypal";
		nodeStart.edges.add(Edge(ButtonOk, &nodeHalt, ActionStart));
		nodeStart.edges.add(Edge(ButtonLeft, &nodeFiring));
		nodeStart.edges.add(Edge(ButtonPlus, &nodeSchedule));

		// Halt furnace
		nodeHalt.label = "Zastavit vypal";
		nodeHalt.edges.add(Edge(ButtonOk, &nodeStart, ActionHalt));

		// Enable / Disable Sound Signalization
		nodeAudio.label = "Zvuk";
		nodeAudio.edges.add(Edge(ButtonOk, &nodeAudio, ActionSound));
		nodeAudio.edges.add(Edge(ButtonLeft, &nodeSettings));
		nodeAudio.edges.add(Edge(ButtonMinus, &nodeTime));

		// Selected Program
		nodeProgram.label = "Program";
		nodeProgram.edges.add(Edge(ButtonOk, &nodeSchedule, ActionSetProgram));
		nodeProgram.edges.add(Edge(ButtonMinus, &nodeProgram, ActionProgramD));
		nodeProgram.edges.add(Edge(ButtonPlus, &nodeProgram, ActionProgramI));
		nodeProgram.edges.add(Edge(ButtonLeft, &nodeSelectProgram));

		// Schedule Now (Set Immediate start)
		nodeSchNow.label = "Teraz";
		nodeSchNow.edges.add(Edge(ButtonOk, &nodeStart, ActionSchNow));
		nodeSchNow.edges.add(Edge(ButtonLeft, &nodeSchedule));
		nodeSchNow.edges.add(Edge(ButtonRight, &nodeSchYear));

		// Schedule Year
		nodeSchYear.label = "Rok";
		nodeSchYear.edges.add(Edge(ButtonLeft, &nodeSchNow));
		nodeSchYear.edges.add(Edge(ButtonRight, &nodeSchMonth));
		nodeSchYear.edges.add(Edge(ButtonPlus, &nodeSchYear, ActionSchYearI));
		nodeSchYear.edges.add(Edge(ButtonMinus, &nodeSchYear, ActionSchYearD));

		// Schedule Month
		nodeSchMonth.label = "Mesiac";
		nodeSchMonth.edges.add(Edge(ButtonLeft, &nodeSchYear));
		nodeSchMonth.edges.add(Edge(ButtonRight, &nodeSchDay));
		nodeSchMonth.edges.add(Edge(ButtonPlus, &nodeSchMonth, ActionSchMonthI));
		nodeSchMonth.edges.add(Edge(ButtonMinus, &nodeSchMonth, ActionSchMonthD));

		// Schedule Day
		nodeSchDay.label = "Den";
		nodeSchDay.edges.add(Edge(ButtonLeft, &nodeSchMonth));
		nodeSchDay.edges.add(Edge(ButtonRight, &nodeSchHours));
		nodeSchDay.edges.add(Edge(ButtonPlus, &nodeSchDay, ActionSchDayI));
		nodeSchDay.edges.add(Edge(ButtonMinus, &nodeSchDay, ActionSchDayD));

		// Schedule Hours
		nodeSchHours.label = "Hodiny";
		nodeSchHours.edges.add(Edge(ButtonLeft, &nodeSchDay));
		nodeSchHours.edges.add(Edge(ButtonRight, &nodeSchMinutes));
		nodeSchHours.edges.add(Edge(ButtonPlus, &nodeSchHours, ActionSchHoursI));
		nodeSchHours.edges.add(Edge(ButtonMinus, &nodeSchHours, ActionSchHoursD));

		// Schedule Minutes
		nodeSchMinutes.label = "Minuty";
		nodeSchMinutes.edges.add(Edge(ButtonLeft, &nodeSchHours));
		nodeSchMinutes.edges.add(Edge(ButtonRight, &nodeSchSeconds));
		nodeSchMinutes.edges.add(Edge(ButtonPlus, &nodeSchMinutes, ActionSchMinutesI));
		nodeSchMinutes.edges.add(Edge(ButtonMinus, &nodeSchMinutes, ActionSchMinutesD));

		// Schedule Seconds
		nodeSchSeconds.label = "Sekundy";
		nodeSchSeconds.edges.add(Edge(ButtonLeft, &nodeSchMinutes));
		nodeSchSeconds.edges.add(Edge(ButtonRight, &nodeSchApply));
		nodeSchSeconds.edges.add(Edge(ButtonPlus, &nodeSchSeconds, ActionSchSecondsI));
		nodeSchSeconds.edges.add(Edge(ButtonMinus, &nodeSchSeconds, ActionSchSecondsD));

		// Schedule Apply settings
		nodeSchApply.label = "Uloz";
		nodeSchApply.edges.add(Edge(ButtonOk, &nodeStart));
		nodeSchApply.edges.add(Edge(ButtonLeft, &nodeSchSeconds));
		nodeSchApply.edges.add(Edge(ButtonRight, &nodeSchYear));

		// Set Date and Time
		nodeTime.label = "Cas a datum";
		nodeTime.edges.add(Edge(ButtonPlus, &nodeAudio));
		nodeTime.edges.add(Edge(ButtonLeft, &nodeSettings));
		nodeTime.edges.add(Edge(ButtonRight, &nodeTimeYear));

		// Date Year
		nodeTimeYear.label = "Rok";
		nodeTimeYear.edges.add(Edge(ButtonLeft, &nodeTime));
		nodeTimeYear.edges.add(Edge(ButtonRight, &nodeTimeMonth));
		nodeTimeYear.edges.add(Edge(ButtonPlus, &nodeTimeYear, ActionTimeYearI));
		nodeTimeYear.edges.add(Edge(ButtonMinus, &nodeTimeYear, ActionTimeYearD));

		// Date Month
		nodeTimeMonth.label = "Mesiac";
		nodeTimeMonth.edges.add(Edge(ButtonLeft, &nodeTimeYear));
		nodeTimeMonth.edges.add(Edge(ButtonRight, &nodeTimeDay));
		nodeTimeMonth.edges.add(Edge(ButtonPlus, &nodeTimeMonth, ActionTimeMonthI));
		nodeTimeMonth.edges.add(Edge(ButtonMinus, &nodeTimeMonth, ActionTimeMonthD));

		// Date Day
		nodeTimeDay.label = "Den";
		nodeTimeDay.edges.add(Edge(ButtonLeft, &nodeTimeMonth));
		nodeTimeDay.edges.add(Edge(ButtonRight, &nodeTimeHours));
		nodeTimeDay.edges.add(Edge(ButtonPlus, &nodeTimeDay, ActionTimeDayI));
		nodeTimeDay.edges.add(Edge(ButtonMinus, &nodeTimeDay, ActionTimeDayD));

		// Time Hours
		nodeTimeHours.label = "Hodiny";
		nodeTimeHours.edges.add(Edge(ButtonLeft, &nodeTimeDay));
		nodeTimeHours.edges.add(Edge(ButtonRight, &nodeTimeMinutes));
		nodeTimeHours.edges.add(Edge(ButtonPlus, &nodeTimeHours, ActionTimeHoursI));
		nodeTimeHours.edges.add(Edge(ButtonMinus, &nodeTimeHours, ActionTimeHoursD));

		// Time Minutes
		nodeTimeMinutes.label = "Miuty";
		nodeTimeMinutes.edges.add(Edge(ButtonLeft, &nodeTimeHours));
		nodeTimeMinutes.edges.add(Edge(ButtonRight, &nodeTimeSeconds));
		nodeTimeMinutes.edges.add(Edge(ButtonPlus, &nodeTimeMinutes, ActionTimeMinutesI));
		nodeTimeMinutes.edges.add(Edge(ButtonMinus, &nodeTimeMinutes, ActionTimeMinutesD));

		// Time Seconds
		nodeTimeSeconds.label = "Sekundy";
		nodeTimeSeconds.edges.add(Edge(ButtonLeft, &nodeTimeMinutes));
		nodeTimeSeconds.edges.add(Edge(ButtonRight, &nodeTimeApply));
		nodeTimeSeconds.edges.add(Edge(ButtonPlus, &nodeTimeSeconds, ActionTimeSecondsI));
		nodeTimeSeconds.edges.add(Edge(ButtonMinus, &nodeTimeSeconds, ActionTimeSecondsD));

		// Set current time
		nodeTimeApply.label = "Uloz";
		nodeTimeApply.edges.add(Edge(ButtonLeft, &nodeTimeSeconds, ActionNone));
		nodeTimeApply.edges.add(Edge(ButtonRight, &nodeTimeYear, ActionNone));
		nodeTimeApply.edges.add(Edge(ButtonOk, &nodeTime, ActionTimeApply));

		// Set root node
		Current = &nodeMenu;
	}

	// Key-Press Event Handler
	void OnKeyPress(Button pressed)
	{
		for (int i = 0; i < Current->edges.size(); i++)
		{
			if (Current->edges.get(i).button == pressed)
			{
				Edge e = Current->edges.get(i);
				DoAction(e.action);
				Current = e.target;
				DrawOnDisplay();
				return;
			}
		}
		DoAction(ActionFailSignal);
		DrawOnDisplay();
	}

	void JumpToHaltNode() { Current = &nodeHalt; }
	void JumpToMenuNode() { Current = &nodeMenu; }
};