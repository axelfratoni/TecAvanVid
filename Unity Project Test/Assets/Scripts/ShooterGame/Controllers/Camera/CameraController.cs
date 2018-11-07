using UnityEngine;

namespace ShooterGame.Camera
{
    public class CameraController : MonoBehaviour
    {
        private GameObject _player;
        private readonly Vector3 _offset = new Vector3(0, 0.6f, 0);

        void LateUpdate () {
            if (_player != null)
            {
                transform.position = _player.transform.position + _offset;
                transform.rotation = _player.transform.rotation;
            }
        }

        public void SetPlayer(GameObject player)
        {
            _player = player;
        }

        public void PlayerDeath()
        {
            _player.GetComponent<PlayerHealth>().PlayDeathSong();
            _player = null;
            transform.position += transform.forward * -2f + new Vector3(0, 2f, 0);
            transform.Rotate(45, 0, 0);
        }
    }
}