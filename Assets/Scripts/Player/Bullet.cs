using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace BrawlShooter
{
    public class Bullet : NetworkBehaviour
    {
        private float _speed;
        private byte _damage;
        private float _range;

        private Vector3 _initPos;

        [SerializeField]
        private float _hitRadius = 0.1f;

        [SerializeField]
        private LayerMask _hitMask;

        private List<LagCompensatedHit> _hits = new List<LagCompensatedHit>();

        private bool _destroyed;

        public void InitNetworkState(byte damage, float speed, float range, Vector3 aimDirection)
        {
            _damage = damage;
            _speed = speed;
            _range = range;

            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, aimDirection.y), Vector3.up);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

        public override void Spawned()
        {
            _initPos = transform.position;
            _destroyed = false;
        }

        public override void FixedUpdateNetwork()
        {
            if (_destroyed)
                return;

            transform.position += _speed * transform.forward * Runner.DeltaTime;

            int hitsCount = Runner.LagCompensation.OverlapSphere(transform.position, _hitRadius, Object.InputAuthority, _hits, _hitMask, HitOptions.SubtickAccuracy);
            if (hitsCount > 0)
            {
                for (int i = 0; i < hitsCount; i++)
                {
                    GameObject other = _hits[i].GameObject;
                    if (other)
                    {
                        Debug.Log("Hit " + other.name);
                        IDamageable target = other.GetComponent<IDamageable>();
                        if (target != null)
                        {
                            target.ApplyDamage(_damage, Object.InputAuthority);
                        }

                        Runner.Despawn(Object);
                        _destroyed = true;
                        return;
                    }
                }
            }

            if (Vector3.Distance(_initPos, transform.position) > _range)
            {
                Runner.Despawn(Object);
                _destroyed = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!Object.HasStateAuthority || _destroyed)
                return;

            Runner.Despawn(Object);
            _destroyed = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _hitRadius);
        }
#endif
    }
}