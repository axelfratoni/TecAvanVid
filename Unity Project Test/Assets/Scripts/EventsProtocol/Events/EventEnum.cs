using System;
using System.Collections.Generic;
using Events.Actions;

public enum EventEnum
{
	Connection = 0,
	Snapshot = 1,
	CreationRequest = 2,
	Creation = 3,
	Movement = 4,
	AssignPlayer = 5,
	Special = 6,
	ReceiveDamage = 7,
	Rotation = 8
}