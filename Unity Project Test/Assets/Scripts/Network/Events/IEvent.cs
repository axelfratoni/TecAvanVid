using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Libs;
using UnityEngine;

public abstract class IEvent
{
	public abstract byte[] GetByteArray();
	protected EventEnum eventEnum;
	protected BitBuffer buffer;
	protected int seq_id;
	protected int id;
	protected static int seqBitsRequired;
	protected static int idBitsRequired;

	static IEvent()
	{
		seqBitsRequired = BitBuffer.GetBitsRequired(Int32.MaxValue);
		idBitsRequired = BitBuffer.GetBitsRequired(Int32.MaxValue);
	}
	
	public int GetSeqId()
	{
		return seq_id;
	}
	
	public int GetId()
	{
		return id;
	}
	
	public EventEnum GetEventEnum()
	{
		return eventEnum;
	}

	public abstract void Process(GameObject gameObject);

	public abstract int GetBitsRequired();

}
