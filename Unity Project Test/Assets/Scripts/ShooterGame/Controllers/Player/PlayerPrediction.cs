using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Events;
using Events.Actions;
using Libs;
using UnityEngine;

namespace ShooterGame.Controllers
{
    public class PlayerPrediction : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private PlayerSnapshot _playerSnapshot;
        private readonly LinkedList<TimeStampedItem<Vector3>> _movementBuffer = new LinkedList<TimeStampedItem<Vector3>>();
        private double _lastSnapTime;
        private double _lastAckTime;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerSnapshot = GetComponent<PlayerSnapshot>();
        }

        public void ApplyInput(Dictionary<InputEnum, bool> inputMap, float mouseX, float timeStamp)
        {
            _playerMovement.ApplyInput(inputMap);
            Vector3 movement = _playerMovement.GetMovement();
            Vector3 directedMovement = transform.forward * movement.z + transform.right * movement.x;
            bool walking = !directedMovement.magnitude.Equals(0);
            if (walking)
            {
                TimeStampedItem<Vector3> timeStampedMovement = new TimeStampedItem<Vector3>(timeStamp, directedMovement);
                _movementBuffer.AddLast(timeStampedMovement);
            }
        }

        public void ApplySnapshot(double time, Vector3 position, Quaternion rotation, double lastAckInput)
        {
            if (time < _lastSnapTime) { return; }

            bool isSameAck = Math.Abs(_lastAckTime - lastAckInput) < 0.01;
            if (isSameAck)
            {
                lastAckInput += time - _lastSnapTime;
            }
            else
            {            
                _lastAckTime = lastAckInput;
                _lastSnapTime = time;
            }
            
            while (_movementBuffer.First != null && !(_movementBuffer.First.Value.Time > lastAckInput))
            {
                _movementBuffer.RemoveFirst();
            }
            
            foreach (var movement in _movementBuffer)
            {
                position += movement.Item;
            }
            
            _playerSnapshot.AddSnapshot((float) time, position);
        }
    }
}