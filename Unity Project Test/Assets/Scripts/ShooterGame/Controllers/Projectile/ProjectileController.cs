using Events.Actions;
using UnityEngine;

namespace ShooterGame.Controllers.Projectile
{
    public class ProjectileController : ObjectController
    {
        public float ThrowForce = 400;
        public float TimeToExplosion = 1.5f;
        public GameObject ExplosionPrefab;
        
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
                Explode();
            }
        }

        private void Explode()
        {
            GameObject explosion = Instantiate(ExplosionPrefab);
            explosion.transform.position = transform.position;
            Destroy(gameObject);
            Destroy(explosion, 1);
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