using System;
using UnityEditor;
using UnityEngine;

namespace ShooterGame.Controllers.Projectile
{
    public class ProjectileExplosion : MonoBehaviour
    {
        public float TimeToExplosion = 1.5f;
        public float ExplosionRadius = 3;
        public int ExplosionDamage = 100;
        public GameObject ExplosionPrefab;
        
        private float _elapsedTime;
        private int _shootableMask;
        private Action<int> _explosionWatcher;
        
        private void Awake()
        {
            _shootableMask = LayerMask.GetMask ("Shootable");
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > TimeToExplosion)
            {
                Explode();
            }
        }

        public void SetExplosionWatcher(Action<int> explosionWatcher)
        {
            _explosionWatcher = explosionWatcher;
        }

        public void Explode()
        {
            GameObject explosion = Instantiate(ExplosionPrefab);
            explosion.transform.position = transform.position;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, _shootableMask);
            foreach (Collider collider in hitColliders)
            {
                PlayerHealth enemyHealth = collider.GetComponent <PlayerHealth> ();
                if(enemyHealth != null)
                {
                    enemyHealth.TakeDamage(ExplosionDamage);
                }
            }

            if(_explosionWatcher != null) _explosionWatcher(GetComponent<ProjectileController>().ObjectId);
            Destroy(gameObject);
            Destroy(explosion, 1);
        }
    }
}