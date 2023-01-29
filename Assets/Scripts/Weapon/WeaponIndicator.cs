using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrawlShooter
{
    public class WeaponIndicator : MonoBehaviour
    {
        [SerializeField] protected Transform _target;

        protected Vector3 _offset;

        [SerializeField] private LayerMask _whatIsEnvironment;

        private Canvas _canvas;
        private RectTransform _rectTransform;

        private bool _isCalculateRange;
        private float _range;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            FollowTarget();
            UpdateRange();
        }

        private void FollowTarget()
        {
            if (_target == null)
                return;

            transform.position = _target.position + _offset;
        }

        private void UpdateRange()
        {
            if (!_isCalculateRange) return;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up, out hit, _range, _whatIsEnvironment))
            {
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, Vector3.Distance(transform.position, hit.point));
            }
            else
            {
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _range);
            }
        }

        public void Activate(bool v)
        {
            _canvas.enabled = v;
            _isCalculateRange = v;
        }

        public virtual void SetTarget(Transform target, Vector3 offset)
        {
            _target = target;
            _offset = offset;
        }

        public void SetRange(float range)
        {
            _range = range;
        }

        public void SetDirection(Vector2 aimDirection)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, aimDirection.y), Vector3.up);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.up * _range);
        }
#endif
    }
}