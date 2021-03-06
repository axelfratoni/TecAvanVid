using System;
using Events;
using Events.Actions;
using UnityEngine;

namespace ShooterGame.Controllers
{
    public class PlayerSnapshot : MonoBehaviour
    {
        private Animator _anim;
        private Rigidbody _playerRigidBody;
        private Vector3 _position;
        private Quaternion _rotation;
        private readonly NetworkBuffer<Vector3> _positionBuffer = new NetworkBuffer<Vector3>();
        private readonly NetworkBuffer<Quaternion> _rotationBuffer = new NetworkBuffer<Quaternion>();
	
        private void Awake ()
        {
            _anim = GetComponent<Animator>();
            _playerRigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Vector3 previousPosition = _playerRigidBody.position;
            
            if (_positionBuffer.GetNextItem(Vector3.Lerp, Time.deltaTime, out _position))
                _playerRigidBody.MovePosition(_position);
            if (_rotationBuffer.GetNextItem(Quaternion.Lerp, Time.deltaTime, out _rotation))
                _playerRigidBody.MoveRotation(_rotation.normalized);
            
            bool walking = Math.Abs(_position.x - previousPosition.x) > 0.01 || 
                           Math.Abs(_position.z - previousPosition.z) > 0.01;
            _anim.SetBool ("IsWalking", walking);
        }

        public void AddSnapshot(double time, Vector3 position, Quaternion rotation)
        {
            _positionBuffer.AddItem(position, (float) time);
            _rotationBuffer.AddItem(rotation, (float) time);
        }

        public void AddSnapshot(double time, Vector3 position)
        {
            _positionBuffer.AddItem(position, (float) time);
        }
    }
}