using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BrawlShooter
{
    [RequireComponent(typeof(RectangleMeshBuilder))]
    public class AimIndicator : MonoBehaviour
    {
        public Transform follow;
        public float range;

        [SerializeField]
        protected Vector3 _offset;

        [SerializeField] 
        private LayerMask _obstacleMask;

        private RectangleMeshBuilder _indicatorView;

        private void Awake()
        {
            _indicatorView = GetComponent<RectangleMeshBuilder>();
        }

        private void LateUpdate()
        {
            FollowTarget();
        }

        private void FollowTarget()
        {
            if (follow == null) return;

            transform.position = follow.position + _offset;
        }

        public void ShowIndicator(bool value)
        {
            _indicatorView.gameObject.SetActive(value);
        }

        public void UpdateIndicator(Vector2 aimDirection)
        {
            if (!_indicatorView.gameObject.activeInHierarchy) return;

            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, aimDirection.y), Vector3.up);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up, out hit, range, _obstacleMask))
            {
                _indicatorView.viewLength = hit.distance;
            }
            else
            {
                _indicatorView.viewLength = range;
            }
        }
    }
}