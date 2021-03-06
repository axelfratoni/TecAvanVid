﻿using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Events;
using Events.Actions;
using Libs;
using ShooterGame.Controllers;
using ShooterGame.Controllers.Projectile;
using UnityEngine;
using Event = Events.Event;

public class ServerManager : MonoBehaviour {

	public GameObject PlayerPrefab;
	public GameObject ProjectilePrefab;
	public int ServerPort = 10000;
	public int SnapshotInterval = 100;
	public int Delay;
	public float PacketLoss;

	private EventManager _eventManager;
	private ObjectIdManager _objectIdManager;
	private SnapshotManager _snapshotManager;
	private TimeManager _timeManager;
	private List<ObjectController> _objects = new List<ObjectController>();

	private void Start () 
	{
		_objectIdManager = new ObjectIdManager();
		_snapshotManager = new SnapshotManager(SnapshotInterval);
		_eventManager = new EventManager(ServerPort, null, Delay, PacketLoss);
		_timeManager = new TimeManager(SnapshotAction.MaxCycleTime);
	}

	private void Update () 
	{
		_timeManager.UpdateTime(Time.deltaTime);
		HandlePendingEvents();
		SendSnapshots();
	}

	private void HandlePendingEvents()
	{
		Event iEvent;
		while (_eventManager.GetNextPendingEvent(out iEvent))
		{
			switch (iEvent.GetEventEnum())
			{
				case EventEnum.Connection:
					ProcessConnection(iEvent.ClientId);
					break;
				case EventEnum.CreationRequest:
					((CreationRequestAction)iEvent.GetPayload()).Extract(ProcessCreationRequest, iEvent.ClientId);
					break;
				case EventEnum.Movement:
					((MovementAction)iEvent.GetPayload()).Extract(ProcessInput, iEvent.ClientId);
					break;
				case EventEnum.Rotation:
					((RotationAction)iEvent.GetPayload()).Extract(ProcessRotation, iEvent.ClientId);
					break;
			}
		}
	}

	private void SendSnapshots()
	{
		if (_snapshotManager.ShouldSendSnapshot())
		{
			float timeStamp = _timeManager.GetCurrentTime();
			_objects.ForEach(obj =>
			{
				Vector3 position = obj.transform.position;
				Quaternion rotation = obj.transform.rotation;
				double lastInput = obj.LastClientInputTime;

				_eventManager.BroadcastEventAction(new SnapshotAction(obj.ObjectId, position, rotation, timeStamp, lastInput));
			});
			
			_snapshotManager.SetFlag(false);
		}
	}

	private void HealthWatcher(int currentHealth, int objectId, int clientId)
	{
		_eventManager.BroadcastEventAction(new DamageAction(currentHealth, objectId));
		if (currentHealth == 0)
		{
			_objects = _objects.Where(player => !player.ObjectId.Equals(objectId)).ToList();
		}
	}

	private void ExplosionWatcher(int objectId)
	{
		_objects = _objects.Where(obj => !obj.ObjectId.Equals(objectId)).ToList();
		_eventManager.BroadcastEventAction(new SpecialAction(SpecialActionEnum.Destroy, objectId));
	}

	private void ProcessConnection(int clientId)
	{
		_objects.ForEach(player =>
		{
			Vector3 position = player.transform.position;
			_eventManager.SendEventAction(new CreationAction(position, player.ObjectId, ObjectEnum.Player), clientId);
		});
	}
	
	private void ProcessInput(double time, Dictionary<InputEnum, bool> inputMap, int clientId, Quaternion rotation) 
	{
		//Debug.Log("Received input" + InputMapper.InputMapToInt(inputMap));
		PlayerController playerController = (PlayerController) _objects.Find(player => player.ClientId.Equals(clientId) &&
																					   player.ObjectType.Equals(ObjectEnum.Player));
		if (playerController != null)
		{
			playerController.ApplyRotation(rotation);
			playerController.ApplyInput(time, inputMap);
			bool isFiring;
			inputMap.TryGetValue(InputEnum.ClickLeft, out isFiring);
			if (playerController.IsFiring() != isFiring)
			{
				playerController.SetFiring(isFiring);
				_eventManager.BroadcastEventAction(new SpecialAction(isFiring? SpecialActionEnum.FiringStart : 
																			   SpecialActionEnum.FiringStop,
																		playerController.ObjectId));
			}

			bool isThrowingProjectile;
			if (inputMap.TryGetValue(InputEnum.ClickRight, out isThrowingProjectile) && isThrowingProjectile)
			{
				ShootProjectile(playerController, clientId);
			}
			
		}
	}

	private void ShootProjectile(PlayerController shooter, int clientId)
	{
		int objectId = _objectIdManager.GetNext();
		Vector3 forwardDirection = shooter.gameObject.transform.forward;
		Vector3 creationPosition = shooter.gameObject.transform.position + forwardDirection * 1 + new Vector3(0, 1f, 0);
		ProjectileController projectileController = Instantiate(ProjectilePrefab).GetComponent<ProjectileController>();
		projectileController.InitializeServer(objectId, clientId, creationPosition, forwardDirection, ExplosionWatcher);
		_objects.Add(projectileController);
		
		_eventManager.BroadcastEventAction(new CreationAction(creationPosition, objectId, ObjectEnum.Projectile));
	}

	public void ProcessCreationRequest(int clientId, Vector3 creationPosition, ObjectEnum objectType)
	{
		Debug.Log("Creation request received from client " + clientId + " " + objectType +" at " + creationPosition);
		if (objectType.Equals(ObjectEnum.Player))
		{
			int objectId = _objectIdManager.GetNext();
			PlayerController playerController = Instantiate(PlayerPrefab).GetComponent<PlayerController>();
			playerController.InitializeServer(objectId, clientId, creationPosition, HealthWatcher);
			_objects.Add(playerController);
			
			_eventManager.BroadcastEventAction(new CreationAction(creationPosition, objectId, objectType));
			_eventManager.SendEventAction(new AssignPlayerAction(objectId), clientId);
			
		}
	}

	public void ProcessRotation(int clientId, Quaternion rotation)
	{
		PlayerController playerController = (PlayerController) _objects.Find(player => player.ClientId.Equals(clientId) &&
		                                                                               player.ObjectType.Equals(ObjectEnum.Player));
		if (playerController != null)
		{
			playerController.ApplyRotation(rotation);
		}
	}
	
	private void OnDisable()
	{
		_eventManager.Disable();
		_snapshotManager.Disable();
	}

	private class ObjectIdManager
	{
		private int _nextObjectId;

		public int GetNext()
		{
			return _nextObjectId++;
		}
	}
	
	private class SnapshotManager
	{
		private bool _shouldSendSnapshot;
		private readonly Timer _timer;

		public SnapshotManager(double timeInterval)
		{
			_timer = new Timer();
			_timer.Elapsed += delegate { _shouldSendSnapshot = true; };
			_timer.Interval = timeInterval;
			_timer.Enabled = true;
		}

		public bool ShouldSendSnapshot()
		{
			return _shouldSendSnapshot;
		}

		public void SetFlag(bool shouldSendSnapshot)
		{
			_shouldSendSnapshot = shouldSendSnapshot;
		}

		public void Disable()
		{
			_timer.Dispose();
		}
	}
}


