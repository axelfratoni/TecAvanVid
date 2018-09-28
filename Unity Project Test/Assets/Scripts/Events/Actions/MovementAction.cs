using System;
using System.Collections.Generic;
using Libs;
using UnityEngine;

namespace Events.Actions
{
    public class MovementAction : EventAction
    {
        private double _time;
        private InputEnum _input;

        public MovementAction(double time, InputEnum input)
        {
            _time = time;
            _input = input;
        }
        
        public MovementAction(BitBuffer payload)
        {
            _time = payload.readFloat( 0.0f, 3600.0f, 0.01f);
            _input = (InputEnum) payload.readInt(0, Enum.GetValues(typeof(InputEnum)).Length);
        }

        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeFloat((float) _time, 0.0f, 3600.0f, 0.01f);
            buffer.writeInt((int) _input, 0, Enum.GetValues(typeof(InputEnum)).Length);
        }

        public override void Execute(WorldManager worldManager)
        {
            worldManager.ProcessInput(_time, _input);
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