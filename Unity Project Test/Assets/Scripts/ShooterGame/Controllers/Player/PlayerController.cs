using System;
using System.Collections.Generic;
using Events.Actions;
using UnityEngine;
using UnityEngine.Analytics;

namespace ShooterGame.Controllers
{
    public class PlayerController : ObjectController
    {
        private PlayerMovement _playerMovement;
        private PlayerShooting _playerShooting;
        private PlayerSnapshot _playerSnapshot;
        private PlayerHealth _playerHealth;
        private PlayerPrediction _playerPrediction;
        private int _shootableMask;
        private Rigidbody _rigidBody;


        public PlayerController()
        {
            ObjectType = ObjectEnum.Player; 
        }

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerShooting = GetComponentInChildren<PlayerShooting>();
            _playerSnapshot = GetComponent<PlayerSnapshot>();
            _playerHealth = GetComponent<PlayerHealth>();
            _playerPrediction = GetComponent<PlayerPrediction>();
            _shootableMask = LayerMask.GetMask ("Shootable");
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Initialize(int objectId, int clientId, Vector3 initialPosition)
        {
            ClientId = clientId;
            ObjectId = objectId;
            _rigidBody.MovePosition(initialPosition);
        }

        public void InitializeServer(int objectId, int clientId, Vector3 initialPosition, Action<int, int, int> healthWatcher)
        {
            Initialize(objectId, clientId, initialPosition);
            _playerHealth.SetHealthWatcher(healthWatcher);
            
            SetUpComponents(false, false);
        }
        
        public void InitializeClient(int objectId, int clientId, Vector3 initialPosition, bool prediction)
        {
            Initialize(objectId, clientId, initialPosition);
            ToggleInputSnapshotController(false);
            SetShootableLayer(false);

            SetUpComponents(true, prediction);
        }

        public void SetUpComponents(bool isClient, bool hasPrediction)
        {
            _playerMovement.enabled = !isClient;
            _playerSnapshot.enabled = isClient;
            _playerPrediction.enabled = isClient && hasPrediction;
        }

        public void ToggleInputSnapshotController(bool isControlledByInput)
        {
            _playerSnapshot.enabled = !isControlledByInput;
            _playerMovement.enabled = isControlledByInput;
        }

        public void ApplyInput(double time, Dictionary<InputEnum, bool> inputMap)
        {
            _playerMovement.ApplyInput(inputMap);
            LastClientInputTime = (float) time;
        }

        public void ApplyMouseInput(double mouseX)
        {
            _playerMovement.SetRotation((float) mouseX);
        }

        public void ApplySnapshot(double time, Vector3 position, Quaternion rotation)
        {
            _playerSnapshot.AddSnapshot(time, position, rotation);
        }
        
        public void ApplySnapshot(double time, Vector3 position)
        {
            _playerSnapshot.AddSnapshot(time, position);
        }
        
        public void ApplyInputPrediction(Dictionary<InputEnum, bool> inputMap, float mouseX, double timeStamp)
        {
            _playerPrediction.ApplyInput(inputMap, mouseX, (float) timeStamp);
        }
        
        public void ApplySnapshotPrediction(double time, Vector3 position, Quaternion rotation, double lastACKInput)
        {
            _playerPrediction.ApplySnapshot(time, position, rotation, lastACKInput);
        }

        public void SetFiring(bool firing)
        {
            _playerShooting.SetFiring(firing);
        }

        public bool IsFiring()
        {
            return _playerShooting.GetFiring();
        }

        public void SetShootableLayer(bool isShootable)
        {
            gameObject.layer = isShootable ? _shootableMask : 0;
        }

        public void UpdateHealth(int health)
        {
            _playerHealth.UpdateHealth(health);
        }

        public Quaternion GetRotation()
        {
            return _rigidBody.rotation;
        }

        public void ApplyRotation(Quaternion rotation)
        {
            _rigidBody.MoveRotation(rotation.normalized);
        }
    }
}