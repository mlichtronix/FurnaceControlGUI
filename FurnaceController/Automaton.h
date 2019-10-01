#pragma once
#include "WString.h"
#include <stdio.h>
#include "Button.h"

enum Action
{
	ActionNone = 0,						// No Action required
	ActionStart, ActionHalt,			// Start / Halt furnace
	ActionP1, ActionP2, ActionP3, ActionP4, ActionP5, ActionP6, ActionP7,		// Set Program #
	ActionTimeYearI, ActionTimeMonthI, ActionTimeDayI, ActionTimeHoursI, ActionTimeMinutesI, ActionTimeSecondsI,		// Set real time - Increase
	ActionTimeYearD, ActionTimeMonthD, ActionTimeDayD, ActionTimeHoursD, ActionTimeMinutesD, ActionTimeSecondsD,		// Set real time - Decrease
	ActionApplyRtc,						// Apply RealTimeClock Values
	ActionSchedNow,						// Set RTC as Start Time
	ActionSchedYearI, ActionSchedMonthI, ActionSchedDayI, ActionSchedHoursI, ActionSchedMinutesI, ActionSchedSecondsI,		// Set Schedule time - Increase
	ActionSchedYearD, ActionSchedMonthD, ActionSchedDayD, ActionSchedHoursD, ActionSchedMinutesD, ActionSchedSecondsD,		// Set Schedule time - Decrease
	ActionSound, ActionFailSignal		// Enable / Disable audio signalization
};

class Node;		// Declaration for recurent inclusion (Chicken-Egg dilemma)
class Edge
{
public:
	Node * target;
	Action action;
	Button button;
	Edge() {};
	Edge(Button b, Node * t, Action a)
	{
		target = t;
		action = a;
		button = b;
	}
	~Edge() { delete target; }
};

class Node
{
public:
	String label;
	LinkedList<Edge> edges;
	Node()
	{
		edges = LinkedList<Edge>();
	};
	Node::~Node() {};
};

class Automaton
{
private:
	DateTime clockTmp;		// Temp variable for updating RTC
	DateTime scheduleTmp;	// Temp variable for updating Schedule

	void DoAction(Action a)
	{
		switch (a)
		{
			// Operation
			case ActionNone: return;									// Do nothing (No key was pressed)
			case ActionFailSignal: PlaySound("FailKey.vaw"); return;	// Play Sound when wronk key pressed
			case ActionStart: { ScheduleTime = scheduleTmp; halted = false; } return;	// Set Begin Time and Start
			case ActionHalt: HaltAndReset(); return;					// Halt Furnace and reset
			case ActionSound: enableAudio = !enableAudio; return;
			// Select Predefined Program
			case ActionP1: SetPredefinedProgram(0); return;		// Program 1
			case ActionP2: SetPredefinedProgram(1); return;		// Program 2
			case ActionP3: SetPredefinedProgram(2); return;		// Program 3
			case ActionP4: SetPredefinedProgram(3); return;		// program 4
			case ActionP5: SetPredefinedProgram(4); return;		// Program 5
			case ActionP6: SetPredefinedProgram(5); return;		// Program 6
			case ActionP7: SetPredefinedProgram(6); return;		// program 7
			// RTC
			case ActionTimeYearI: clockTmp.Year = RotateUp(clockTmp.Year, 2019, 2050); return;
			case ActionTimeYearD: clockTmp.Year = RotateDown(clockTmp.Year, 2019, 2050); return;
			case ActionTimeMonthI: clockTmp.Month = RotateUp(clockTmp.Month, 1, 12); return;
			case ActionTimeMonthD: clockTmp.Month = RotateDown(clockTmp.Month, 1, 12); return;
			case ActionTimeDayI: clockTmp.Day = RotateUp(clockTmp.Day, 1, 31); return;
			case ActionTimeDayD: clockTmp.Day = RotateDown(clockTmp.Day, 1, 31); return;
			case ActionTimeHoursI: clockTmp.Hours = RotateUp(clockTmp.Hours, 0, 23); return;
			case ActionTimeHoursD: clockTmp.Hours = RotateDown(clockTmp.Hours, 0, 23); return;
			case ActionTimeMinutesI: clockTmp.Minutes = RotateUp(clockTmp.Minutes, 0, 59); return;
			case ActionTimeMinutesD: clockTmp.Minutes = RotateDown(clockTmp.Minutes, 0, 59); return;
			case ActionTimeSecondsI: clockTmp.Seconds = RotateUp(clockTmp.Seconds, 0, 59); return;
			case ActionTimeSecondsD: clockTmp.Seconds = RotateDown(clockTmp.Seconds, 0, 59); return;
			case ActionApplyRtc: SetRealTime(clockTmp); return;		// Set Real Time Clock
			// Schedule
			case ActionSchedNow: scheduleTmp = rtc.Now(); return;	// Set Schedule to Current Time
			case ActionSchedYearI: scheduleTmp.Year = RotateUp(scheduleTmp.Year, 2019, 2050); return;
			case ActionSchedYearD: scheduleTmp.Year = RotateDown(scheduleTmp.Year, 2019, 2050); return;
			case ActionSchedMonthI: scheduleTmp.Month = RotateUp(scheduleTmp.Month, 1, 12); return;
			case ActionSchedMonthD: scheduleTmp.Month = RotateDown(scheduleTmp.Month, 1, 12); return;
			case ActionSchedDayI: scheduleTmp.Day = RotateUp(scheduleTmp.Day, 1, 31); return;
			case ActionSchedDayD: scheduleTmp.Day = RotateDown(scheduleTmp.Day, 1, 31); return;
			case ActionSchedHoursI: scheduleTmp.Hours = RotateUp(scheduleTmp.Hours, 0, 23); return;
			case ActionSchedHoursD: scheduleTmp.Hours = RotateDown(scheduleTmp.Hours, 0, 23); return;
			case ActionSchedMinutesI: scheduleTmp.Minutes = RotateUp(scheduleTmp.Minutes, 0, 59); return;
			case ActionSchedMinutesD: scheduleTmp.Minutes = RotateDown(scheduleTmp.Minutes, 0, 59); return;
			case ActionSchedSecondsI: scheduleTmp.Seconds = RotateUp(scheduleTmp.Seconds, 0, 59); return;
			case ActionSchedSecondsD: scheduleTmp.Seconds = RotateDown(scheduleTmp.Seconds, 0, 59); return;
			// Something went wrong
			default: SendMessage(Error, "Undefined action! [" + String(a) + "]"); break;
		}
	}

	int RotateUp(int val, int min, int max)
	{
		return val++ < max ? val : min;
	}

	int RotateDown(int val, int min, int max)
	{
		return val-- > min ? val : max;
	}

public:
	Node * Current;

	Automaton()
	{
		// Create Node Set
		Node nodeMenu, nodeFiring, nodeSettings, nodeProgram, nodeSchedule, nodeStart, nodeHalt;
		Node nodeP1, nodeP2, nodeP3, nodeP4, nodeP5, nodeP6, nodeP7;
		Node nodeSchNow, nodeSchYear, nodeSchMonth, nodeSchDay, nodeSchHours, nodeSchMinutes, nodeSchSeconds;
		Node nodeSchApply, nodeTimeApply, nodeAudio;
		Node nodeTime, nodeTimeYear, nodeTimeMonth, nodeTimeDay, nodeTimeHours, nodeTimeMinutes, nodeTimeSeconds;

		// Create Edge Set and initialize Localized Labels

		// Idle Menu
		nodeMenu.label = "Menu";
		nodeMenu.edges.add(Edge(ButtonRight, &nodeFiring, ActionNone));

		// Firing
		nodeFiring.label = "Palenie";
		nodeFiring.edges.add(Edge(ButtonRight, &nodeProgram, ActionNone));
		nodeFiring.edges.add(Edge(ButtonMinus, &nodeSettings, ActionNone)),

		// Settings
		nodeSettings.label = "Nastavenia";
		nodeSettings.edges.add(Edge(ButtonRight, &nodeAudio, ActionNone));
		nodeSettings.edges.add(Edge(ButtonLeft, &nodeMenu, ActionNone));
		nodeSettings.edges.add(Edge(ButtonPlus, &nodeFiring, ActionNone));

		// Select Program
		nodeProgram.label = "Volba programu";
		nodeProgram.edges.add(Edge(ButtonRight, &nodeP1, ActionNone));
		nodeProgram.edges.add(Edge(ButtonLeft, &nodeFiring, ActionNone));
		nodeProgram.edges.add(Edge(ButtonMinus, &nodeSchedule, ActionNone));

		// Schedule Time
		nodeSchedule.label = "Cas zaciatku";
		nodeSchedule.edges.add(Edge(ButtonRight, &nodeSchYear, ActionNone));
		nodeSchedule.edges.add(Edge(ButtonLeft, &nodeFiring, ActionNone));
		nodeSchedule.edges.add(Edge(ButtonMinus, &nodeStart, ActionNone));
		nodeSchedule.edges.add(Edge(ButtonPlus, &nodeProgram, ActionNone));

		// Start
		nodeStart.label = "Spustit vypal";
		nodeStart.edges.add(Edge(ButtonOk, &nodeHalt, ActionStart));
		nodeStart.edges.add(Edge(ButtonLeft, &nodeFiring, ActionNone));
		nodeStart.edges.add(Edge(ButtonPlus, &nodeSchedule, ActionNone));

		// Halt furnace
		nodeHalt.label = "Zastavit vypal";
		nodeHalt.edges.add(Edge(ButtonOk, &nodeStart, ActionHalt));

		// Program 1
		nodeP1.label = SplitString(predefined[0], '|').get(0);
		nodeP1.edges.add(Edge(ButtonOk, &nodeSchedule, ActionP1));
		nodeP1.edges.add(Edge(ButtonMinus, &nodeP7, ActionNone));
		nodeP1.edges.add(Edge(ButtonPlus, &nodeP2, ActionNone));
		nodeP1.edges.add(Edge(ButtonLeft, &nodeProgram, ActionNone));

		// Program 2
		nodeP2.label = SplitString(predefined[1], '|').get(0);
		nodeP2.edges.add(Edge(ButtonOk, &nodeSchedule, ActionP2));
		nodeP2.edges.add(Edge(ButtonMinus, &nodeP1, ActionNone));
		nodeP2.edges.add(Edge(ButtonPlus, &nodeP3, ActionNone));
		nodeP2.edges.add(Edge(ButtonLeft, &nodeProgram, ActionNone));

		// Program 3
		nodeP3.label = SplitString(predefined[2], '|').get(0);
		nodeP3.edges.add(Edge(ButtonOk, &nodeSchedule, ActionP3));
		nodeP3.edges.add(Edge(ButtonMinus, &nodeP2, ActionNone));
		nodeP3.edges.add(Edge(ButtonPlus, &nodeP4, ActionNone));
		nodeP3.edges.add(Edge(ButtonLeft, &nodeProgram, ActionNone));

		// Program 4
		nodeP4.label = SplitString(predefined[2], '|').get(0);
		nodeP4.edges.add(Edge(ButtonOk, &nodeSchedule, ActionP4));
		nodeP4.edges.add(Edge(ButtonMinus, &nodeP3, ActionNone));
		nodeP4.edges.add(Edge(ButtonPlus, &nodeP5, ActionNone));
		nodeP4.edges.add(Edge(ButtonLeft, &nodeProgram, ActionNone));

		// Program 5
		nodeP5.label = SplitString(predefined[2], '|').get(0);
		nodeP5.edges.add(Edge(ButtonOk, &nodeSchedule, ActionP5));
		nodeP5.edges.add(Edge(ButtonMinus, &nodeP4, ActionNone));
		nodeP5.edges.add(Edge(ButtonPlus, &nodeP6, ActionNone));
		nodeP5.edges.add(Edge(ButtonLeft, &nodeProgram, ActionNone));

		// Program 6
		nodeP6.label = SplitString(predefined[2], '|').get(0);
		nodeP6.edges.add(Edge(ButtonOk, &nodeSchedule, ActionP6));
		nodeP6.edges.add(Edge(ButtonMinus, &nodeP5, ActionNone));
		nodeP6.edges.add(Edge(ButtonPlus, &nodeP7, ActionNone));
		nodeP6.edges.add(Edge(ButtonLeft, &nodeProgram, ActionNone));

		// Program 7
		nodeP7.label = SplitString(predefined[2], '|').get(0);
		nodeP7.edges.add(Edge(ButtonOk, &nodeSchedule, ActionP7));
		nodeP7.edges.add(Edge(ButtonMinus, &nodeP6, ActionNone));
		nodeP7.edges.add(Edge(ButtonPlus, &nodeP1, ActionNone));
		nodeP7.edges.add(Edge(ButtonLeft, &nodeProgram, ActionNone));

		// TODO:
		nodeSchNow.label = "Teraz";
		nodeSchYear.label = "";
		nodeSchMonth.label = "";
		nodeSchDay.label = "";
		nodeSchHours.label = "";
		nodeSchMinutes.label = "";
		nodeSchSeconds.label = "";
		nodeSchApply.label = "Uloz";
		nodeAudio.label = "Zvuk";
		nodeTime.label = "Cas";
		nodeTimeYear.label = "";
		nodeTimeMonth.label = "";
		nodeTimeDay.label = "";
		nodeTimeHours.label = "";
		nodeTimeMinutes.label = "";
		nodeTimeSeconds.label = "";
		nodeTimeApply.label = "Uloz";
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
				return;
			}
		}
		DoAction(ActionFailSignal);
	}
};