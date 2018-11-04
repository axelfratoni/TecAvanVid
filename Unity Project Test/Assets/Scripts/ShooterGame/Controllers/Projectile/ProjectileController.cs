using System;
using Events.Actions;
using UnityEngine;

namespace ShooterGame.Controllers.Projectile
{
    public class ProjectileController : ObjectController
    {
        public float ThrowForce = 400;
        
        private Rigidbody _rigidBody;
        private ProjectileExplosion _projectileExplosion;
        private ProjectileSnapshot _projectileSnapshot;

        public ProjectileController()
        {
            ObjectType = ObjectEnum.Projectile;
        }
        
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _projectileExplosion = GetComponent<ProjectileExplosion>();
            _projectileSnapshot = GetComponent<ProjectileSnapshot>();
        }

        private void Initialize(int objectId, int clientId, Vector3 creationPosition)
        {
            ClientId = clientId;
            ObjectId = objectId;
            _rigidBody.MovePosition(creationPosition);
        }
        
        public void InitializeServer(int objectId, int clientId, Vector3 creationPosition, Vector3 forward, Action<int> explosionWatcher)
        {
            Initialize(objectId, clientId, creationPosition);
            
            _projectileExplosion.enabled = true;
            _projectileSnapshot.enabled = false;
            
            _rigidBody.AddForce((forward + new Vector3(0,1,0)) * ThrowForce );
            
            _projectileExplosion.SetExplosionWatcher(explosionWatcher);
        }

        public void InitializeClient(int objectId, int clientId, Vector3 creationPosition)
        {
            Initialize(objectId, clientId, creationPosition);

            _projectileExplosion.enabled = false;
            _projectileSnapshot.enabled = true;

            _rigidBody.useGravity = false;
            _rigidBody.isKinematic = true;
        }

        public void ApplySnapshot(double time, Vector3 position)
        {
            _projectileSnapshot.AddSnapshot(time, position);
        }

        public void Explode()
        {
            _projectileExplosion.enabled = true;
            _projectileExplosion.TimeToExplosion = 0;
        }
    }
}