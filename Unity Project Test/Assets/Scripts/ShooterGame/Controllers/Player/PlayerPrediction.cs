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
        private Vector3 _lastPredictedWithInput;
        private Vector3 _lastPredictedWithSnapshot;
        private bool _receivedSnapshot;
        private Rigidbody _rigidBody;
        private Animator _anim;
        private double _lastACKTime = 0;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _playerMovement = GetComponent<PlayerMovement>();
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_receivedSnapshot)
            {
                _receivedSnapshot = false;
                _rigidBody.MovePosition(_lastPredictedWithSnapshot);
            }
            else
            {
                _rigidBody.MovePosition(_lastPredictedWithInput);
            }
        }

        public void ApplyInput(Dictionary<InputEnum, bool> inputMap, float mouseX, float timeStamp)
        {
            _playerMovement.ApplyInput(inputMap);
            Vector3 movement = _playerMovement.GetMovement();
            Vector3 directedMovement = transform.forward * movement.z + transform.right * movement.x;
            if (!directedMovement.magnitude.Equals(0))
            {
                TimeStampedItem<Vector3> timeStampedMovement = new TimeStampedItem<Vector3>(timeStamp, directedMovement);
                _movementBuffer.AddLast(timeStampedMovement);
                Vector3 nextPosition = transform.position + directedMovement;
                //_rigidBody.MovePosition(nextPosition);
                _lastPredictedWithInput = nextPosition;
            }

            
            //Debug.Log("Predicted " + nextPosition + " at " + timeStamp);
            
            bool walking = Math.Abs(directedMovement.x) > 0.01 || 
                           Math.Abs(directedMovement.z) > 0.01;
            _anim.SetBool ("IsWalking", walking);
        }

        public void ApplySnapshot(double time, Vector3 position, Quaternion rotation, double lastAckInput)
        {
            if (lastAckInput <= _lastACKTime) { return; }
            
            //Debug.Log("Ack time " + lastAckInput);
            //Debug.Log("Last time " + _movementBuffer.Last.Value.Time);
            //if(_movementBuffer.Last != null)Debug.Log("Last input " + _movementBuffer.Last.Value.Time);
            
            //Debug.Log("Before filter " + _movementBuffer.Count);
            while (_movementBuffer.First != null && !(_movementBuffer.First.Value.Time > lastAckInput))
            {
                _movementBuffer.RemoveFirst();
            }
            //Debug.Log("After filter " + _movementBuffer.Count);
            
            foreach (var movement in _movementBuffer)
            {
                position += movement.Item;
            }
            _lastPredictedWithSnapshot = position;
            _receivedSnapshot = true;
            _lastACKTime = lastAckInput;
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