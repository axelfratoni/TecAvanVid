using Network;
using UnityEngine;

namespace Controllers
{
    public class ProjectileController : MonoBehaviour
    {
        private Projectile _projectile;
        private bool _hasProjectileStarted;

        private void FixedUpdate()
        {
            if (!_hasProjectileStarted && _projectile != null)
            {
                transform.position = _projectile.position;
                _hasProjectileStarted = true;
            }
        }

        public void SetProjectile(Projectile projectile)
        {
            _projectile = projectile;
        }
    }
}