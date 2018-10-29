using System.Collections.Generic;
using Events.Actions;
using UnityEngine;
using UnityEngine.Analytics;

namespace ShooterGame.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public int ClientId { get; private set; }
        public int ObjectId { get; private set; }
        
        private PlayerMovement _playerMovement;
        private PlayerShooting _playerShooting;
        private PlayerSnapshot _playerSnapshot;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerShooting = GetComponentInChildren<PlayerShooting>();
            _playerSnapshot = GetComponent<PlayerSnapshot>();

            _playerSnapshot.enabled = false;
        }

        public void Initialize(int objectId, int clientId, Vector3 initialPosition)
        {
            ClientId = clientId;
            ObjectId = objectId;
            _playerSnapshot.SetPosition(initialPosition);
        }

        public void ToggleInputSnapshotController(bool isControlledByInput)
        {
            _playerSnapshot.enabled = !isControlledByInput;
            _playerMovement.enabled = isControlledByInput;
        }

        public void ApplyInput(double time, double mouseX, List<InputEnum> inputList)
        {
            float v = 0; 
            float h = 0;
            bool fire = false;
            inputList.ForEach(input =>
            {
                v += input.Equals(InputEnum.W) ? 1 : input.Equals(InputEnum.S) ? -1 : 0;
                h += input.Equals(InputEnum.D) ? 1 : input.Equals(InputEnum.A) ? -1 : 0;
                fire = input.Equals(InputEnum.ClickLeft) || fire;
            });
            
            _playerMovement.SetMovementAndRotation(h, v, (float)mouseX);
            _playerShooting.SetFiring(fire);
        }

        public void ApplySnapshot(double time, Vector3 position, Quaternion rotation)
        {
            _playerSnapshot.AddSnapshot(time, position, rotation);
        }

        public void SetFiring(bool firing)
        {
            _playerShooting.SetFiring(firing);
        }

        public bool IsFiring()
        {
            return _playerShooting.GetFiring();
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