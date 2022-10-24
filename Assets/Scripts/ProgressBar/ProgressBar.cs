using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrawlShooter
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] protected Transform _target;

        protected Vector3 _offset;

        public bool isWorldSpace;

        [SerializeField] protected Image _progressImage;

        protected float _currentValue, _maxValue;

        protected virtual void Update()
        {
            FollowTarget();
            LookAtCamera();
        }

        protected virtual void FollowTarget()
        {
            if (_target == null)
                return;

            transform.position = _target.position + _offset;
        }

        protected virtual void LookAtCamera()
        {
            if (!isWorldSpace)
                return;

            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.down);
        }

        public virtual void SetTarget(Transform target, Vector3 offset)
        {
            _target = target;
            _offset = offset;
        }

        public virtual void SetProgress(float currentValue, float maxValue)
        {
            _progressImage.fillAmount = currentValue / maxValue;
        }
    }
}
