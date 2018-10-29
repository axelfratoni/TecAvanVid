using System;
using UnityEngine;

namespace ShooterGame.Controllers
{
    public class PlayerSnapshot : MonoBehaviour
    {
        private Animator _anim;
        private Rigidbody _playerRigidBody;
        private Vector3 _position;
        private Quaternion _rotation;
	
        private void Awake ()
        {
            _anim = GetComponent<Animator>();
            _playerRigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            bool walking = Math.Abs(_position.x - _playerRigidBody.position.x) > 0.01 || 
                           Math.Abs(_position.z - _playerRigidBody.position.z) > 0.01;
            _anim.SetBool ("IsWalking", walking);
            
            _playerRigidBody.MovePosition(_position);
            _playerRigidBody.MoveRotation(_rotation.normalized);
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        public void AddSnapshot(double time, Vector3 position, Quaternion rotation)
        {
            _position = position;
            _rotation = rotation;
        }
    }
}