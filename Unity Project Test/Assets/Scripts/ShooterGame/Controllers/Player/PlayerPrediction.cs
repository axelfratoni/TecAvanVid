using System;
using System.Collections.Generic;
using Events.Actions;
using Libs;
using UnityEngine;

namespace ShooterGame.Controllers
{
    public class PlayerPrediction : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private TimeManager _timeManager;
        private readonly LinkedList<TimeStampedItem<Move>> _movementBuffer = new LinkedList<TimeStampedItem<Move>>();
        private TimeStampedItem<Move> _lastPredictedPosition;
        private Rigidbody _rigidBody;
        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _playerMovement = GetComponent<PlayerMovement>();
            _rigidBody = GetComponent<Rigidbody>();
            _timeManager = new TimeManager(SnapshotAction.MaxCycleTime);
        }

        private void Update()
        {
            _timeManager.UpdateTime(Time.deltaTime);
            if (_lastPredictedPosition != null)
            {
                bool walking = Math.Abs(_lastPredictedPosition.Item.Movement.x - transform.position.x) > 0.01 || 
                               Math.Abs(_lastPredictedPosition.Item.Movement.z - transform.position.z) > 0.01;
                _anim.SetBool ("IsWalking", walking);

                Vector3 movement = Vector3.Lerp(transform.position, _lastPredictedPosition.Item.Movement, Time.deltaTime * _playerMovement.Speed);
                _rigidBody.MovePosition(movement);
                Quaternion rotation = Quaternion.Lerp(transform.rotation, _lastPredictedPosition.Item.Rotation, Time.deltaTime * _playerMovement.MouseSensitivity);
                _rigidBody.MoveRotation(rotation.normalized);
            }
        }

        public void ApplyInput(Dictionary<InputEnum, bool> inputMap, float mouseX)
        {
            float timeStamp = _timeManager.GetCurrentTime();
            
            _playerMovement.ApplyInput(inputMap);
            Vector3 movement = _playerMovement.GetMovement();

            Quaternion rotation = _playerMovement.CalculateRotation(mouseX);
            
            Move move = new Move(movement, rotation);
            TimeStampedItem<Move> nextMove = new TimeStampedItem<Move>(timeStamp, move);
            _movementBuffer.AddLast(nextMove);
            
            Vector3 forward = _lastPredictedPosition.Item.Rotation * Vector3.forward;
            Vector3 right = _lastPredictedPosition.Item.Rotation * Vector3.right;
            Vector3 nextPosition = _lastPredictedPosition.Item.Movement + forward * movement.z + right * movement.x;
            Quaternion nextRotation = _lastPredictedPosition.Item.Rotation * rotation;
            
            Move nextPrediction = new Move(nextPosition, nextRotation);
            _lastPredictedPosition = new TimeStampedItem<Move>(timeStamp, nextPrediction);
        }

        public void ApplySnapshot(double time, Vector3 position, Quaternion rotation)
        {
            while (_movementBuffer.First != null && _movementBuffer.First.Value.Time < time)
            {
                _movementBuffer.RemoveFirst();
            }

            if (_lastPredictedPosition == null || _movementBuffer.Count == 0)
            {
                _timeManager.SetElapsedTime((float) time);
                _movementBuffer.Clear();
            }

            foreach (var timeStampedItem in _movementBuffer)
            {
                Vector3 forward = rotation * Vector3.forward;
                Vector3 right = rotation * Vector3.right;
                position += forward * timeStampedItem.Item.Movement.z + right * timeStampedItem.Item.Movement.x;
                rotation *= timeStampedItem.Item.Rotation;
            }
            Move predictedPosition = new Move(position, rotation);
            _lastPredictedPosition = new TimeStampedItem<Move>((float) time, predictedPosition);
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