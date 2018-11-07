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
		Vector3 movement = CalculateMovement(_inputMap);
		SetMovement(movement);
	}

	public void ApplyInput(Dictionary<InputEnum, bool> inputMap)
	{
		bool isPressed;
		if(inputMap.TryGetValue(InputEnum.A, out isPressed)) _inputMap[InputEnum.A] = isPressed;
		if(inputMap.TryGetValue(InputEnum.S, out isPressed)) _inputMap[InputEnum.S] = isPressed;
		if(inputMap.TryGetValue(InputEnum.D, out isPressed)) _inputMap[InputEnum.D] = isPressed;
		if(inputMap.TryGetValue(InputEnum.W, out isPressed)) _inputMap[InputEnum.W] = isPressed;
	}

	private Vector3 CalculateMovement(Dictionary<InputEnum, bool> inputMap)
	{
		float v = 0; 
		float h = 0;
		bool isPressed;
		if(inputMap.TryGetValue(InputEnum.A, out isPressed)) h += isPressed ? -1 : 0;
		if(inputMap.TryGetValue(InputEnum.S, out isPressed)) v += isPressed ? -1 : 0;
		if(inputMap.TryGetValue(InputEnum.D, out isPressed)) h += isPressed ? 1 : 0;
		if(inputMap.TryGetValue(InputEnum.W, out isPressed)) v += isPressed ? 1 : 0;
		
		Vector3 movement =  new Vector3(h, 0, v).normalized * Speed * Time.deltaTime;
		return movement;
	}

	public Vector3 GetMovement()
	{
		return CalculateMovement(_inputMap);
	}

	public Quaternion CalculateRotation(float mouseX)
	{
		return Quaternion.Euler(new Vector3(0, mouseX * MouseSensitivity, 0));
	}
	
	public void SetPosition(Vector3 position)
	{
		_playerRigidBody.MovePosition(position);
	}

	public void SetMovement(Vector3 movement)
	{
		_playerRigidBody.MovePosition(transform.position + transform.forward * movement.z + transform.right * movement.x);

		bool walking = Math.Abs(movement.x) > 0.01 || Math.Abs(movement.z) > 0.01;
		_anim.SetBool ("IsWalking", walking);
	}

	public void SetRotation(float mouseX)
	{
		Quaternion rotation = CalculateRotation(mouseX);
		_playerRigidBody.MoveRotation(_playerRigidBody.rotation * rotation);
	}
}
