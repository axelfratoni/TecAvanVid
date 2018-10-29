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
    }
}