using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Events.Actions;
using Libs;
using UnityEngine;

namespace ShooterGame.Controllers
{
    public class PlayerPrediction : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private readonly LinkedList<TimeStampedItem<Vector3>> _movementBuffer = new LinkedList<TimeStampedItem<Vector3>>();
        private Rigidbody _rigidBody;
        private Animator _anim;
        private double _lastSnapTime;
        private double _lastAckTime;
        private Vector3 _lastPredictedPosition;
        private bool _hasReceivedSnap;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _playerMovement = GetComponent<PlayerMovement>();
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_hasReceivedSnap)
            {
                _hasReceivedSnap = false;
                _rigidBody.MovePosition(_lastPredictedPosition);
            }
            else
            {
                _playerMovement.SetMovement(_playerMovement.GetMovement());
            }
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
                Vector3 nextPosition = transform.position + directedMovement;
                //_rigidBody.MovePosition(nextPosition);
            }
            _anim.SetBool ("IsWalking", walking);
        }

        public void ApplySnapshot(double time, Vector3 position, Quaternion rotation, double lastAckInput)
        {
            if (time < _lastSnapTime) { return; }
            
            bool isSameAck = _lastAckTime >= lastAckInput;
            if (isSameAck)
            {
                _lastAckTime += time - _lastSnapTime;
            }
            else
            {            
                _lastAckTime = lastAckInput;
            }
            
            while (_movementBuffer.First != null && !(_movementBuffer.First.Value.Time > _lastAckTime))
            {
                _movementBuffer.RemoveFirst();
            }
            
            foreach (var movement in _movementBuffer)
            {
                position += movement.Item;
            }
            
            //Debug.Log("Buffer " + _movementBuffer.Count);
            //if(_movementBuffer.First != null)Debug.Log("First " + _movementBuffer.First.Value.Time);
            //Debug.Log("Last ack " + lastAckInput);
            //_rigidBody.MovePosition(position);
            _lastSnapTime = time;
            _hasReceivedSnap = true;
            _lastPredictedPosition = position;
        }
        
        private class Move
        {
            public Vector3 Movement { get; private set; }
            public Quaternion Rotation { get; private set; }
            
            internal Move(Vector3 movement, Quaternion rotation)
            {
                Movement = movement;
                Rotation = rotation;
            }
        }
    }
}