using System;
using System.Collections;
using System.Collections.Generic;
using Events.Actions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float MouseSensitivity = 2F;
	public float Speed = 10f;
	
	private Animator _anim;
	private Rigidbody _playerRigidBody;
	private Dictionary<InputEnum, bool> _inputMap;
	
	private void Awake ()
	{
		_anim = GetComponent<Animator>();
		_playerRigidBody = GetComponent<Rigidbody>();
		_inputMap = new Dictionary<InputEnum, bool>
		{
			{InputEnum.A, false}, {InputEnum.S, false}, {InputEnum.D, false}, {InputEnum.W, false}
		};
	}

	private void Update()
	{
		float v = 0; 
		float h = 0;
		bool isPressed;
		if(_inputMap.TryGetValue(InputEnum.A, out isPressed)) h += isPressed ? -1 : 0;
		if(_inputMap.TryGetValue(InputEnum.S, out isPressed)) v += isPressed ? -1 : 0;
		if(_inputMap.TryGetValue(InputEnum.D, out isPressed)) h += isPressed ? 1 : 0;
		if(_inputMap.TryGetValue(InputEnum.W, out isPressed)) v += isPressed ? 1 : 0;
		
		SetMovement(h, v);
	}

	public void ApplyInput(Dictionary<InputEnum, bool> inputMap)
	{
		bool isPressed;
		if(inputMap.TryGetValue(InputEnum.A, out isPressed)) _inputMap[InputEnum.A] = isPressed;
		if(inputMap.TryGetValue(InputEnum.S, out isPressed)) _inputMap[InputEnum.S] = isPressed;
		if(inputMap.TryGetValue(InputEnum.D, out isPressed)) _inputMap[InputEnum.D] = isPressed;
		if(inputMap.TryGetValue(InputEnum.W, out isPressed)) _inputMap[InputEnum.W] = isPressed;
	}
	
	public void SetPosition(Vector3 position)
	{
		_playerRigidBody.MovePosition(position);
	}

	public void SetMovement(float h, float v)
	{
		Vector3 movement =  new Vector3(h, 0, v).normalized * Speed * Time.deltaTime;
		
		Move(movement);
		Animate(movement);
	}

	public void SetRotation(float mouseX)
	{
		Quaternion rotation = Quaternion.Euler(new Vector3(0, mouseX * MouseSensitivity, 0));
		Rotate(rotation);
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
