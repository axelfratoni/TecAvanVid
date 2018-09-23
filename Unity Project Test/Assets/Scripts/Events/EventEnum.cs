using System;
using System.Collections.Generic;
using Events.Actions;

public enum EventEnum
{
	Connection = 0,
	Snapshot = 1,
	Creation = 2,
	Movement = 3,
	Color = 4,
}

public class EventBitsRequired
{
	private readonly Dictionary<EventEnum, int> _bitsRequired;
	
	public EventBitsRequired()
	{
		_bitsRequired = new Dictionary<EventEnum, int>
		{
			{EventEnum.Connection, 0},
			{EventEnum.Snapshot, SnapshotAction.GetPayloadBitSize()},
			{EventEnum.Creation, CreationAction.GetPayloadBitSize()},
			{EventEnum.Movement, MovementAction.GetPayloadBitSize()},
			{EventEnum.Color, ColorAction.GetPayloadBitSize()}
		};
	}

	public int GetBitsRequired(EventEnum eventEnum)
	{
		int bitsRequired;
		if (!_bitsRequired.TryGetValue(eventEnum, out bitsRequired))
		{
			throw new Exception("This event type doesn't specify its bits required");
		}

		return bitsRequired;
	}
}