using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Libs;
using UnityEngine;

public abstract class IEvent : ICloneable
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
	
	public EventEnum GetEventEnum()
	{
		return eventEnum;
	}

	public abstract void Process(GameObject gameObject);

	public abstract int GetBitsRequired();

	public int SeqId
	{
		get { return seq_id; }
		set { seq_id = value; }
	}

	public int Id
	{
		get { return id; }
		set { id = value; }
	}

	public abstract object Clone();
}
