#pragma once
#include "Action.h"
#include "ScreenType.h"
#include "LinkedList.h"
#include "WString.h"

class State;	// Declaration for recurent inclusion (Chicken-Egg dilemma)
class Edge
{
public:
	State * target;
	Action action;
	Button button;

	Edge() {};
	Edge(Button b, State * t, Action a)
	{
		target = t;
		action = a;
		button = b;
	};
	Edge(Button b, State * t)
	{
		target = t;
		button = b;
		action = ActionNone;
	};
	~Edge() {};
};

class State
{
private:
	int value;
	int MaxValue;
	int MinValue;

public:
	String label;
	LinkedList<Edge> edges;
	ScreenType type;
	State() {};
	State(ScreenType t)
	{
		type = t;
		edges = LinkedList<Edge>();
		value = MaxValue = MinValue = 0;
	};
	State(ScreenType st, int val, int min, int max)
	{
		type = st;
		value = val;
		MaxValue = max;
		MinValue = min;
		edges = LinkedList<Edge>();
	}
	~State() { edges.clear(); };

	void RotateUp() { value = value++ < MaxValue ? value : MinValue; }		// Rotate value up trough min/max bounds
	void RotateDown() { value = value-- > MinValue ? value : MaxValue; }	// Rotate value down trough min/max bounds
	void SetValue(int v) { value = v; }
	int GetValue() { return value; }
};