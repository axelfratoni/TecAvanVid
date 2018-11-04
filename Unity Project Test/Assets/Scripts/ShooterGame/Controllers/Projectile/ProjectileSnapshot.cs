using Events;
using UnityEngine;

namespace ShooterGame.Controllers.Projectile
{
    public class ProjectileSnapshot : MonoBehaviour
    {
        private Rigidbody _rigidBody;
        private Vector3 _position;
        private readonly NetworkBuffer<Vector3> _positionBuffer = new NetworkBuffer<Vector3>();
	
        private void Awake ()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {            
            _position = _positionBuffer.GetNextItem(Vector3.Lerp, Time.deltaTime);
            _rigidBody.MovePosition(_position);
        }

        public void AddSnapshot(double time, Vector3 position)
        {
            //_position = position;
            _positionBuffer.AddItem(position, (float) time);
        }
    }
}