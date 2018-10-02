using System;
using System.Collections.Generic;
using Libs;
using UnityEngine;

namespace Events.Actions
{
    public class MovementAction : EventAction
    {
        private readonly double _time;
        private readonly List<InputEnum> _inputList;

        public MovementAction(double time, List<InputEnum> inputList)
        {
            _time = time;
            _inputList = inputList;
        }
        
        public MovementAction(BitBuffer payload)
        {
            _time = payload.readFloat( 0.0f, 3600.0f, 0.01f);
            int inputListLength = payload.readInt(0, Enum.GetValues(typeof(InputEnum)).Length); 
            _inputList = new List<InputEnum>(inputListLength);
            for (int i = 0; i < inputListLength; i++)
            {
                _inputList.Add((InputEnum) payload.readInt(0, Enum.GetValues(typeof(InputEnum)).Length));
            }
        }

        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeFloat((float) _time, 0.0f, 3600.0f, 0.01f);
            buffer.writeInt(_inputList.Count, 0, Enum.GetValues(typeof(InputEnum)).Length); 
            _inputList.ForEach(input => 
                buffer.writeInt((int) input, 0, Enum.GetValues(typeof(InputEnum)).Length));
        }

        public override void Execute(ActionDispatcher actionDispatcher, int clientId)
        {
            actionDispatcher.ProcessInput(_time, _inputList, clientId);
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.NoTimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.Movement;
        }
    }
    
    public enum InputEnum
    {
        W = 0, 
        A = 1, 
        S = 2, 
        D = 3, 
        ClickLeft = 4, 
        ClickRight = 5 
    }

    public static class InputMapper
    {
        public static List<InputEnum> ExtractInput()
        {
            List<InputEnum> inputList = new List<InputEnum>();

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (h < 0)
            {
                inputList.Add(InputEnum.A);
            }
            if (h > 0)
            {
                inputList.Add(InputEnum.D);
            }
            if (v < 0)
            {
                inputList.Add(InputEnum.S);
            }
            if (v > 0)
            {
                inputList.Add(InputEnum.W);
            }
            if (Input.GetMouseButtonDown(0))
            {
                inputList.Add(InputEnum.ClickLeft);
            }
            if (Input.GetMouseButtonDown(1))
            {
                inputList.Add(InputEnum.ClickRight);
            }

            return inputList;
        }
    }
}