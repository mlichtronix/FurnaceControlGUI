#pragma once
#include "Action.h"
#include "ScreenType.h"
#include "LinkedList.h"
#include "WString.h"

class Node;	// Declaration for recurent inclusion (Chicken-Egg dilemma)
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
	};
	Edge(Button b, Node * t)
	{
		target = t;
		button = b;
		action = ActionNone;
	};
	~Edge() {};
};

class Node
{
private:
	int value;
	int MaxValue;
	int MinValue;

public:
	LinkedList<Edge> edges;
	ScreenType type;
	Node() {};
	Node(ScreenType t)
	{
		type = t;
		edges = LinkedList<Edge>();
		value = MaxValue = MinValue = 0;
	};
	Node(ScreenType st, int val, int min, int max)
	{
		type = st;
		value = val;
		MaxValue = max;
		MinValue = min;
		edges = LinkedList<Edge>();
	}
	~Node() { edges.clear(); };

	void RotateUp() { value = value++ < MaxValue ? value : MinValue; }		// Rotate value up trough min/max bounds
	void RotateDown() { value = value-- > MinValue ? value : MaxValue; }	// Rotate value down trough min/max bounds
	void SetValue(int v) { value = v; }
	int GetValue() { return value; }
};