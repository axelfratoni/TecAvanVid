using UnityEngine;

namespace ShooterGame.Controllers.Projectile
{
    public class ProjectileSnapshot : MonoBehaviour
    {
        private Rigidbody _rigidBody;
        private Vector3 _position;
	
        private void Awake ()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _rigidBody.MovePosition(_position);
        }

        public void AddSnapshot(double time, Vector3 position)
        {
            _position = position;
        }
    }
}