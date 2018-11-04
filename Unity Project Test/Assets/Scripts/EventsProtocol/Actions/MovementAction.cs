using System;
using System.Collections.Generic;
using Libs;
using UnityEngine;

namespace Events.Actions
{
    public class MovementAction : EventAction
    {
        private readonly double _time;
        private readonly Dictionary<InputEnum, bool> _inputMap;
        private readonly float _mouseX;

        // The input dictionary represents a key down or up, being true down and false up.
        public MovementAction(double time, float mouseX, Dictionary<InputEnum, bool> inputMap)
        {
            _time = time;
            _inputMap = inputMap;
            _mouseX = mouseX > 10? 10: mouseX < -10? -10: mouseX;
        }
        
        public MovementAction(BitBuffer payload)
        {
            _time = payload.readFloat( 0.0f, 3600.0f, 0.01f);
            
            _mouseX = payload.readFloat(-10f, 10f, 0.1f);
            
            int inputListLength = payload.readInt(0, Enum.GetValues(typeof(InputEnum)).Length); 
            
            _inputMap = new Dictionary<InputEnum, bool>();
            for (int i = 0; i < inputListLength; i++)
            {
                InputEnum input = (InputEnum) payload.readInt(0, Enum.GetValues(typeof(InputEnum)).Length);
                bool keyPress = payload.readBit();
                _inputMap.Add(input, keyPress);
            }
        }

        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeFloat((float) _time, 0.0f, 3600.0f, 0.01f);
            
            buffer.writeFloat(_mouseX, -10f, 10f, 0.1f);
            
            buffer.writeInt(_inputMap.Count, 0, Enum.GetValues(typeof(InputEnum)).Length); 
            
            foreach(KeyValuePair<InputEnum, bool> entry in _inputMap)
            {
                buffer.writeInt((int) entry.Key, 0, Enum.GetValues(typeof(InputEnum)).Length);
                buffer.writeBit(entry.Value);
            }
        }

        public void Extract(Action<double, double, Dictionary<InputEnum, bool>, int> executor, int clientId)
        {
            executor(_time, _mouseX, _inputMap, clientId);
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
        ClickRight = 5,
    }

    public static class InputMapper
    {
        public static Dictionary<InputEnum, bool> ExtractInput()
        {
            Dictionary<InputEnum, bool> inputMap = new Dictionary<InputEnum, bool>();

            if (Input.GetKeyDown(KeyCode.A))
            {
                inputMap.Add(InputEnum.A, true);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                inputMap.Add(InputEnum.S, true);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                inputMap.Add(InputEnum.D, true);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                inputMap.Add(InputEnum.W, true);
            }
            if (Input.GetMouseButtonDown(0)) 
            {
                inputMap.Add(InputEnum.ClickLeft, true);
            }
            if (Input.GetMouseButtonDown(1))
            {
                inputMap.Add(InputEnum.ClickRight, true);
            }
            
            if (Input.GetKeyUp(KeyCode.A))
            {
                inputMap.Add(InputEnum.A, false);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                inputMap.Add(InputEnum.S, false);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                inputMap.Add(InputEnum.D, false);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                inputMap.Add(InputEnum.W, false);
            }
            if (Input.GetMouseButtonUp(0)) 
            {
                inputMap.Add(InputEnum.ClickLeft, false);
            }
            if (Input.GetMouseButtonUp(1))
            {
                inputMap.Add(InputEnum.ClickRight, false);
            }

            return inputMap;
        }
    }
}