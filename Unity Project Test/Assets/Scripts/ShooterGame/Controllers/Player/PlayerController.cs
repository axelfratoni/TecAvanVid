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

            _playerSnapshot.enabled = false;
        }

        public void Initialize(int objectId, int clientId, Vector3 initialPosition)
        {
            ClientId = clientId;
            ObjectId = objectId;
            _playerSnapshot.SetPosition(initialPosition);
        }

        public void SetHealthWatcher(Action<int, int, int> healthWatcher)
        {
            _playerHealth.SetHealthWatcher(healthWatcher);
        }

        public void ToggleInputSnapshotController(bool isControlledByInput)
        {
            _playerSnapshot.enabled = !isControlledByInput;
            _playerMovement.enabled = isControlledByInput;
        }

        public void ApplyInput(double time, double mouseX, Dictionary<InputEnum, bool> inputMap)
        {
            _playerMovement.ApplyInput(inputMap);
            _playerMovement.SetRotation((float)mouseX);
        }

        public void ApplySnapshot(double time, Vector3 position, Quaternion rotation)
        {
            _playerSnapshot.AddSnapshot(time, position, rotation);
        }

        public void SetFiring(bool firing)
        {
            _playerShooting.SetFiring(firing);
        }

        public void SetShootableLayer(bool isShootable)
        {
            gameObject.layer = isShootable ? 9 : 0;
        }

        public void UpdateHealth(int health)
        {
            _playerHealth.UpdateHealth(health);
        }
    }
    
    /*private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float mouseX = Input.GetAxisRaw("Mouse X");
        bool fire = Input.GetButton("Fire1");
        
        _playerMovement.SetMovementAndRotation(h, v, mouseX);
        _playerShooting.SetFiring(fire);
    }*/
}