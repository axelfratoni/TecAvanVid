using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float MouseSensitivity = 2F;
	public float Speed = 10f;
	
	private Animator _anim;
	private Rigidbody _playerRigidBody;
	
	void Awake ()
	{
		_anim = GetComponent<Animator>();
		_playerRigidBody = GetComponent<Rigidbody>();
	}
	
	public void SetPosition(Vector3 position)
	{
		_playerRigidBody.MovePosition(position);
	}

	public void SetMovementAndRotation(float h, float v, float mouseX)
	{
		Vector3 movement =  new Vector3(h, 0, v).normalized * Speed * Time.deltaTime;
		Quaternion rotation = Quaternion.Euler(new Vector3(0, mouseX * MouseSensitivity, 0));
		
		Move(movement);
		Rotate(rotation);
		Animate(movement);
	}
	
	private void Move(Vector3 movement)
	{
		_playerRigidBody.MovePosition(transform.position + transform.forward * movement.z + transform.right * movement.x);
	}

	private void Rotate(Quaternion rotation)
	{
		_playerRigidBody.MoveRotation(_playerRigidBody.rotation * rotation);
	}
	
	private void Animate (Vector3 movement)
	{
		bool walking = Math.Abs(movement.x) > 0.01 || Math.Abs(movement.z) > 0.01;
		_anim.SetBool ("IsWalking", walking);
	}
}
