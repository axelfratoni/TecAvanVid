using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        private GameObject _player;
        private Vector3 _offset;
	
        void LateUpdate () {
            if (_player != null)
            {
                transform.position = _player.transform.position + _offset;
            }
        }

        public void SetPlayer(GameObject player)
        {
            _player = player;
            _offset = transform.position - _player.transform.position;
        }
    }
}