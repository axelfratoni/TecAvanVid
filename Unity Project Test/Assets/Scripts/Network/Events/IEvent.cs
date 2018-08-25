﻿using System;
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
	
	public int GetSeqId()
	{
		return seq_id;
	}
	
	public EventEnum GetEventEnum()
	{
		return eventEnum;
	}

}