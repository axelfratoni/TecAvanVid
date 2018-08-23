using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Libs;
using UnityEngine;

public abstract class EventInterface
{
	public abstract byte[] GetByteArray();
	protected EventEnum type;
	protected BitBuffer buffer;
	protected bool withTimeout;
	protected int seq_id;
	
	public bool getWithTimeout()
	{
		return withTimeout;
	}
	
	public int getSeqId()
	{
		return seq_id;
	}

}
