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
        //private readonly float _mouseX;

        // The input dictionary represents a key down or up, being true down and false up.
        public MovementAction(double time, float mouseX, Dictionary<InputEnum, bool> inputMap)
        {
            _time = time;
            _inputMap = inputMap;
            //_mouseX = mouseX > 10? 10: mouseX < -10? -10: mouseX;
        }
        
        public MovementAction(BitBuffer payload)
        {
            _time = payload.readFloat( 0.0f, 3600.0f, 0.01f);
            
            //_mouseX = payload.readFloat(-10f, 10f, 0.1f);
            
            //int inputListLength = payload.readInt(0, Enum.GetValues(typeof(InputEnum)).Length); 
            
            _inputMap = new Dictionary<InputEnum, bool>();
            for (int i = 0; i < Enum.GetValues(typeof(InputEnum)).Length; i++)
            {
                //InputEnum input = (InputEnum) payload.readInt(0, Enum.GetValues(typeof(InputEnum)).Length);
                bool keyPress = payload.readBit();
                _inputMap.Add((InputEnum)i, keyPress);
            }
        }

        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeFloat((float) _time, 0.0f, 3600.0f, 0.01f);
            
            //buffer.writeFloat(_mouseX, -10f, 10f, 0.1f);


            //int count = 0;
            for (int i = 0; i < Enum.GetValues(typeof(InputEnum)).Length; i++)
            {
                bool isPressed;
                _inputMap.TryGetValue((InputEnum) i, out isPressed);
                buffer.writeBit(isPressed);
                //count <<= 1;
                //count |= isPressed? 1 : 0;
            }
            //Debug.Log("Input serial " + count);
            
            /*buffer.writeInt(_inputMap.Count, 0, Enum.GetValues(typeof(InputEnum)).Length); 
            
            foreach(KeyValuePair<InputEnum, bool> entry in _inputMap)
            {
                buffer.writeInt((int) entry.Key, 0, Enum.GetValues(typeof(InputEnum)).Length);
                buffer.writeBit(entry.Value);
            }*/
        }

        public void Extract(Action<double, Dictionary<InputEnum, bool>, int> executor, int clientId)
        {
            executor(_time, _inputMap, clientId);
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
            
            inputMap[InputEnum.W] = Input.GetKey(KeyCode.W);
            inputMap[InputEnum.A] = Input.GetKey(KeyCode.A);
            inputMap[InputEnum.S] = Input.GetKey(KeyCode.S);
            inputMap[InputEnum.D] = Input.GetKey(KeyCode.D);
            inputMap[InputEnum.ClickLeft] = Input.GetMouseButton(0);
            inputMap[InputEnum.ClickRight] = Input.GetMouseButton(1);

            return inputMap;
        }

        public static int InputMapToInt(Dictionary<InputEnum, bool> inputMap)
        {
            int count = 0;
            for (int i = 0; i < Enum.GetValues(typeof(InputEnum)).Length; i++)
            {
                bool isPressed;
                inputMap.TryGetValue((InputEnum) i, out isPressed);
                count <<= 1;
                count |= isPressed? 1 : 0;
            }

            return count;
        }

        public static bool CompareInputs(Dictionary<InputEnum, bool> inputMap1, Dictionary<InputEnum, bool> inputMap2)
        {
            return InputMapToInt(inputMap1).Equals(InputMapToInt(inputMap2));
        }
    }
}