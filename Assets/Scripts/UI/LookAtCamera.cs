using UnityEngine;

namespace BrawlShooter
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _camera;

        private void Start()
        {
            _camera = FindObjectOfType<Camera>().transform;
        }

        private void LateUpdate()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - _camera.position);
        }
    }
}
