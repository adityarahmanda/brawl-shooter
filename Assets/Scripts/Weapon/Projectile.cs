using DG.Tweening;
using Fusion;
using Lean.Pool;
using UnityEngine;

namespace BrawlShooter
{
    public class Projectile : MonoBehaviour
    {
        // PUBLIC MEMBERS

        public bool IsFinished { get; private set; }
        public bool IsDiscarded { get; private set; }

        public virtual bool NeedsInterpolationData => false;

        // PRIVATE MEMBERS
        [SerializeField]
        private float _damage = 10f;

        [SerializeField]
        private float _startSpeed = 10f;
        
        [SerializeField]
        private float _maxDistance = 10f;
        
        [SerializeField]
        private float _maxTime = 1f;

        [Header("Interpolation")]
        [SerializeField, Tooltip("Time for interpolation between barrel position and actual fire path of the projectile")]
        private float _interpolationDuration = 0.3f;

        [SerializeField]
        private Ease _interpolationEase = Ease.OutSine;

        private Vector3 _startOffset;
        private float _interpolationTime;

        private int _maxLiveTimeTicks = -1;

        [Header("Impact")]
        [SerializeField]
        private float _impactEffectReturnTime = 2f;

        [SerializeField]
        private GameObject _impactEffectPrefab;

        private TrailRenderer[] _trails;

        [Header("Hit")]
        [SerializeField, Tooltip("Projectile length improves hitting moving targets")]
        private float _hitLength = .5f;

        //[SerializeField]
        //private EHitType _hitType = EHitType.Projectile;

        [SerializeField]
        private LayerMask _hitMask;

        // PUBLIC METHODS

        public ProjectileData GetFireData(NetworkRunner runner, Vector3 firePosition, Vector3 fireDirection, float maxDistance)
        {
            if (_maxLiveTimeTicks < 0)
            {
                _maxDistance = maxDistance;
                int maxDistanceTicks = Mathf.RoundToInt((_maxDistance / _startSpeed) * runner.Simulation.Config.TickRate);
                int maxTimeTicks = Mathf.RoundToInt(_maxTime * runner.Simulation.Config.TickRate);

                // GetFireData is called on prefab directly, but it is safe to save
                // the value here as it does not change for different instances
                _maxLiveTimeTicks = maxDistanceTicks > 0 && maxTimeTicks > 0 ? Mathf.Min(maxDistanceTicks, maxTimeTicks) : (maxDistanceTicks > 0 ? maxDistanceTicks : maxTimeTicks);
            }

            return new ProjectileData()
            {
                FirePosition = firePosition,
                FireVelocity = fireDirection * _startSpeed,
            };
        }

        public void OnFixedUpdate(ProjectileContext context, ref ProjectileData data)
        {
            var previousPosition = GetMovePosition(context.Runner, ref data, context.Runner.Tick - 1);
            var nextPosition = GetMovePosition(context.Runner, ref data, context.Runner.Tick);

            var direction = nextPosition - previousPosition;
            float distance = direction.magnitude;

            // Normalize
            direction /= distance;

            if (_hitLength > 0f)
            {
                float elapsedDistanceSqr = (previousPosition - data.FirePosition).sqrMagnitude;
                float projectileLength = elapsedDistanceSqr > _hitLength * _hitLength ? _hitLength : Mathf.Sqrt(elapsedDistanceSqr);

                previousPosition -= direction * projectileLength;
                distance += projectileLength;
            }

            if (ProjectileUtility.ProjectileCast(context.Runner, context.InputAuthority, previousPosition, direction, distance, _hitMask, out LagCompensatedHit hit) == true)
            {
                //HitUtility.ProcessHit(context.InputAuthority, direction, hit, _damage, _hitType);

                SpawnImpact(context, ref data, hit.Point, (hit.Normal + -direction) * 0.5f);

                data.IsFinished = true;
            }

            if (context.Runner.Tick >= data.FireTick + _maxLiveTimeTicks)
            {
                data.IsFinished = true;
            }
        }

        public void Activate(ProjectileContext context, ref ProjectileData data)
        {
            IsFinished = false;
            IsDiscarded = false;

            OnActivated(context, ref data);
        }

        public void Deactivate(ProjectileContext context)
        {
            IsFinished = true;

            OnDeactivated(context);
        }

        public void OnRender(ProjectileContext context, ref ProjectileData data)
        {
            if (data.IsFinished == true)
            {
                SpawnImpactVisual(context, ref data);
                IsFinished = true;
                return;
            }

            var targetPosition = GetRenderPosition(context, ref data);
            float interpolationProgress = 0f;

            if (targetPosition != data.FirePosition)
            {
                // Do not start interpolation until projectile should actually move
                _interpolationTime += Time.deltaTime;
                interpolationProgress = Mathf.Clamp01(_interpolationTime / _interpolationDuration);
            }

            interpolationProgress = DOVirtual.EasedValue(0f, 1f, interpolationProgress, _interpolationEase);
            var offset = Vector3.Lerp(_startOffset, Vector3.zero, interpolationProgress);

            var previousPosition = transform.position;
            var nextPosition = targetPosition + offset;
            var direction = nextPosition - previousPosition;

            transform.position = nextPosition;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        public void Discard()
        {
            IsDiscarded = true;
            OnDiscarded();
        }

        // MONOBEHAVIOR

        private void Awake()
        {
            _trails = GetComponentsInChildren<TrailRenderer>(true);
        }

        // private METHODS

        private void OnActivated(ProjectileContext context, ref ProjectileData data)
        {
            transform.position = context.BarrelTransform.position;
            transform.rotation = Quaternion.LookRotation(data.FireVelocity);

            _startOffset = context.BarrelTransform.position - data.FirePosition;
            _interpolationTime = 0f;
        }

        private void OnDeactivated(ProjectileContext context)
        {
            // Clear trials before returning to cache
            for (int i = 0; i < _trails.Length; i++)
            {
                _trails[i].Clear();
            }
        }

        private void OnDiscarded()
        {
            IsFinished = true;
        }

        protected void SpawnImpact(ProjectileContext context, ref ProjectileData data, Vector3 position, Vector3 normal)
        {
            if (context.Runner.Stage == default)
            {
                Debug.LogError("Call SpawnImpact only from fixed update");
                return;
            }

            if (position == Vector3.zero)
                return;

            data.ImpactPosition = position;
            data.ImpactNormal = normal;
        }

        private void SpawnImpactVisual(ProjectileContext context, ref ProjectileData data)
        {
            if (context.Runner.Stage != default)
            {
                Debug.LogError("Call SpawnImpactVisual only from render method");
                return;
            }

            if (data.ImpactPosition == Vector3.zero)
                return;

            Debug.Log("Spawn Impact");

            if (_impactEffectPrefab != null)
            {
                var impact = LeanPool.Spawn(_impactEffectPrefab);

                impact.transform.SetPositionAndRotation(data.ImpactPosition, Quaternion.LookRotation(data.ImpactNormal));
                context.Runner.MoveToRunnerScene(impact);

                LeanPool.Despawn(impact, _impactEffectReturnTime);
            }
        }

        private Vector3 GetRenderPosition(ProjectileContext context, ref ProjectileData data)
        {
            return GetMovePosition(context.Runner, ref data, context.FloatTick);
        }

        private Vector3 GetMovePosition(NetworkRunner runner, ref ProjectileData data, float currentTick)
        {
            float time = (currentTick - data.FireTick) * runner.DeltaTime;

            if (time <= 0f)
                return data.FirePosition;

            return data.FirePosition + data.FireVelocity * time;
        }
    }
}
