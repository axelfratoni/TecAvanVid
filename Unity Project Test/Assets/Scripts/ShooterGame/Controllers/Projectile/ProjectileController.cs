using Events.Actions;
using UnityEngine;

namespace ShooterGame.Controllers.Projectile
{
    public class ProjectileController : ObjectController
    {
        public float ThrowForce = 400;
        public float TimeToExplosion = 2;
        
        private Rigidbody _rigidBody;
        private float _elapsedTime;

        public ProjectileController()
        {
            ObjectType = ObjectEnum.Projectile;
        }
        
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > TimeToExplosion)
            {
                Destroy(gameObject);
            }
        }


        public void Initialize(int objectId, int clientId, Vector3  creationPosition, Vector3 forward)
        {
            ClientId = clientId;
            ObjectId = objectId;
            _rigidBody.MovePosition(creationPosition);
            _rigidBody.AddForce((forward + new Vector3(0,1,0)) * ThrowForce );
        }
    }
}