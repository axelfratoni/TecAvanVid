using System.Collections.Generic;
using System.Linq;
using Events;
using Events.Actions;
using ShooterGame.Controllers;
using UnityEngine;
using Event = Events.Event;

public class ServerManager : MonoBehaviour {

	public GameObject PlayerPrefab;
	public int ServerPort = 10000;

	private EventManager _eventManager;
	private ObjectIdManager _objectIdManager;
	private List<PlayerController> _players = new List<PlayerController>();

	private void Start () {
		_objectIdManager = new ObjectIdManager();
		_eventManager = new EventManager(ServerPort, null);
	}

	private void Update () {
		Queue<Event> pendingEvents = _eventManager.GetPendingEvents();
		while (pendingEvents.Count > 0)
		{
			Event iEvent = pendingEvents.Dequeue();
			if(iEvent == null) continue;
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
			}
		}
		
		_players.ForEach(player =>
		{
			Vector3 position = player.transform.position;
			Quaternion rotation = player.transform.rotation;
			_eventManager.BroadcastEventAction(new SnapshotAction(player.ObjectId, position, rotation, 0));
		});
	}

	private void HealthWatcher(int currentHealth, int objectId, int clientId)
	{
		_eventManager.BroadcastEventAction(new DamageAction(currentHealth, objectId));
		if (currentHealth == 0)
		{
			_players = _players.Where(player => !player.ObjectId.Equals(objectId)).ToList();
		}
	}

	private void ProcessConnection(int clientId)
	{
		_players.ForEach(player =>
		{
			Vector3 position = player.transform.position;
			_eventManager.SendEventAction(new CreationAction(position, player.ObjectId, ObjectEnum.Player), clientId);
		});
	}
	
	private void ProcessInput(double time, double mouseX, List<InputEnum> inputList, int clientId) 
	{
		Debug.Log("Received input");
		PlayerController playerController = _players.Find(player => player.ClientId.Equals(clientId));
		if (playerController != null)
		{
			bool isFiring = inputList.Contains(InputEnum.ClickLeft);
			if (isFiring && !playerController.IsFiring() || !isFiring && playerController.IsFiring())
			{
				_eventManager.BroadcastEventAction(new SpecialAction(isFiring? SpecialActionEnum.FiringStart : 
																			   SpecialActionEnum.FiringStop,
																	playerController.ObjectId));
			}
			
			playerController.ApplyInput(time, mouseX, inputList);
		}
	}

	public void ProcessCreationRequest(int clientId, Vector3 creationPosition, ObjectEnum objectType)
	{
		Debug.Log("Creation request received from client " + clientId + " " + objectType +" at " + creationPosition);
		if (objectType.Equals(ObjectEnum.Player))
		{
			int objectId = _objectIdManager.GetNext();
			PlayerController playerController = Instantiate(PlayerPrefab).GetComponent<PlayerController>();
			playerController.Initialize(objectId, clientId, creationPosition);
			playerController.SetHealthWatcher(HealthWatcher);
			_players.Add(playerController);
			
			_eventManager.BroadcastEventAction(new CreationAction(creationPosition, objectId, objectType));
			_eventManager.SendEventAction(new AssignPlayerAction(objectId), clientId);
			
		}
	}
	
	private void OnDisable()
	{
		_eventManager.Disable();
	}

	private class ObjectIdManager
	{
		private int _nextObjectId;

		public int GetNext()
		{
			return _nextObjectId++;
		}
	}
}


