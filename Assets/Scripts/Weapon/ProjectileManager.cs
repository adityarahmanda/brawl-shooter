using Fusion;
using Lean.Pool;
using UnityEngine;

namespace BrawlShooter
{
    public struct ProjectileData : INetworkStruct
     {
        public bool IsActive { get { return _state.IsBitSet(0); } set { _state.SetBit(0, value); } }
        public bool IsFinished { get { return _state.IsBitSet(1); } set { _state.SetBit(1, value); } }

        private byte _state;

        public int FireTick;
        public Vector3 FirePosition;
        public Vector3 FireVelocity;
        [Networked, Accuracy(0.01f)]
        public Vector3 ImpactPosition { get; set; }
        [Networked, Accuracy(0.01f)]
        public Vector3 ImpactNormal { get; set; }
     }

    public struct ProjectileInterpolationData
    {
        public ProjectileData From;
        public ProjectileData To;
        public float Alpha;
    }

    public class ProjectileContext
    {
        public NetworkRunner Runner;
        public PlayerRef InputAuthority;
        public int OwnerObjectInstanceID;

        // Barrel transform represents position from which projectile visuals should fly out
        // (actual projectile fire calculations are usually done from different point, for example camera)
        public Transform BarrelTransform;

        public float FloatTick;
        public bool Interpolate;
        public ProjectileInterpolationData InterpolationData;
    }

    [RequireComponent(typeof(Weapon))]
    public class ProjectileManager : NetworkBehaviour
    {
        // PRIVATE MEMBERS
        private Weapon _weapon;

        [SerializeField]
        private bool _fullProxyPrediction = false;
        [SerializeField]
        private Projectile _projectilePrefab;

        [Networked, Capacity(50)]
        private NetworkArray<ProjectileData> _projectiles { get; }
        [Networked]
        private int _projectileCount { get; set; }

        private Projectile[] _visibleProjectiles = new Projectile[50];
        private int _visibleProjectileCount;

        private ProjectileContext _projectileContext;
        private RawInterpolator _projectilesInterpolator;

        // PUBLIC MEMBERS

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
        }

        public void AddProjectile(Vector3 firePosition, Vector3 direction, float maxDistance)
        {
            var fireData = _projectilePrefab.GetFireData(Runner, firePosition, direction, maxDistance);

            AddProjectile(fireData);
        }

        public void AddProjectile(ProjectileData data)
        {
            data.FireTick = Runner.Tick;
            data.IsActive = true;

            int projectileIndex = _projectileCount % _projectiles.Length;

            var previousData = _projectiles[projectileIndex];
            if (previousData.IsActive == true && previousData.IsFinished == false)
            {
                Debug.LogError("No space for another projectile - projectile buffer should be increased or projectile lives too long");
            }

            _projectiles.Set(projectileIndex, data);

            _projectileCount++;
        }

        public override void Spawned()
        {
            _visibleProjectileCount = _projectileCount;

            _projectileContext = new ProjectileContext()
            {
                Runner = Runner,
                InputAuthority = Object.InputAuthority,
                OwnerObjectInstanceID = gameObject.GetInstanceID(),
            };

            _projectilesInterpolator = GetInterpolator(nameof(_projectiles));
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            for (int i = 0; i < _visibleProjectiles.Length; i++)
            {
                var projectile = _visibleProjectiles[i];
                if (projectile != null)
                {
                    DestroyProjectile(projectile);
                    _visibleProjectiles[i] = null;
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            // Projectile calculations are processed only on input or state authority
            // unless full proxy prediction is turned on
            if (IsProxy == true && _fullProxyPrediction == false)
                return;

            _projectileContext.FloatTick = Runner.Tick;

            for (int i = 0; i < _projectiles.Length; i++)
            {
                var projectileData = _projectiles[i];

                if (projectileData.IsActive == false)
                    continue;
                if (projectileData.IsFinished == true)
                    continue;

                _projectilePrefab.OnFixedUpdate(_projectileContext, ref projectileData);

                _projectiles.Set(i, projectileData);
            }
        }

        public override void Render()
        {
            // Visuals are not spawned on dedicated server at all
            if (Runner.Mode == SimulationModes.Server)
                return;

            _projectilesInterpolator.TryGetArray(_projectiles, out var fromProjectiles, out var toProjectiles, out float interpolationAlpha);

            var simulation = Runner.Simulation;
            bool interpolate = IsProxy == true && _fullProxyPrediction == false;

            if (interpolate == true)
            {
                // For proxies we move projectile in snapshot interpolated time
                _projectileContext.FloatTick = simulation.InterpFrom.Tick + (simulation.InterpTo.Tick - simulation.InterpFrom.Tick) * simulation.InterpAlpha;
            }
            else
            {
                _projectileContext.FloatTick = simulation.Tick + simulation.StateAlpha;
            }

            int bufferLength = _projectiles.Length;

            // Our predicted projectiles were not confirmed by the server, let's discard them
            for (int i = _projectileCount; i < _visibleProjectileCount; i++)
            {
                var projectile = _visibleProjectiles[i % bufferLength];
                if (projectile != null)
                {
                    // We are not destroying projectile immediately,
                    // projectile can decide itself how to react
                    projectile.Discard();
                }
            }

            int minFireTick = Runner.Tick - (int)(Runner.Simulation.Config.TickRate * 0.5f);

            // Let's spawn missing projectile gameobjects
            for (int i = _visibleProjectileCount; i < _projectileCount; i++)
            {
                int index = i % bufferLength;
                var projectileData = _projectiles[index];

                // Projectile is long time finished, do not spawn visuals for it
                // Note: We cannot check just IsFinished, because some projectiles are finished
                // immediately in one tick but the visuals could be longer running
                if (projectileData.IsFinished == true && projectileData.FireTick < minFireTick)
                    continue;

                if (_visibleProjectiles[index] != null)
                {
                    Debug.LogError("No space for another projectile gameobject - projectile buffer should be increased or projectile lives too long");
                    DestroyProjectile(_visibleProjectiles[index]);
                }

                _projectileContext.BarrelTransform = _weapon.BarrelTransform;
                _visibleProjectiles[index] = CreateProjectile(_projectileContext, ref projectileData);
            }

            // Update all visible projectiles
            for (int i = 0; i < bufferLength; i++)
            {
                var projectile = _visibleProjectiles[i];
                if (projectile == null)
                    continue;

                if (projectile.IsDiscarded == false)
                {
                    var data = _projectiles[i];

                    bool interpolateProjectile = interpolate == true && projectile.NeedsInterpolationData;

                    // Prepare interpolation data if needed
                    ProjectileInterpolationData interpolationData = default;
                    if (interpolateProjectile == true)
                    {
                        interpolationData.From = fromProjectiles.Get(i);
                        interpolationData.To = toProjectiles.Get(i);
                        interpolationData.Alpha = interpolationAlpha;
                    }

                    _projectileContext.Interpolate = interpolateProjectile;
                    _projectileContext.InterpolationData = interpolationData;

                    // If barrel transform is not available anymore (e.g. weapon was switched before projectile finished)
                    // let's use at least some dummy (first) one. Doesn't matter at this point much.
                    _projectileContext.BarrelTransform = _weapon.BarrelTransform;

                    projectile.OnRender(_projectileContext, ref data);
                }

                if (projectile.IsFinished == true)
                {
                    DestroyProjectile(projectile);
                    _visibleProjectiles[i] = null;
                }
            }

            _visibleProjectileCount = _projectileCount;
        }

        private Projectile CreateProjectile(ProjectileContext context, ref ProjectileData data)
        {
            var projectile = LeanPool.Spawn(_projectilePrefab);

            Runner.MoveToRunnerScene(projectile);

            projectile.Activate(context, ref data);

            return projectile;
        }

        private void DestroyProjectile(Projectile projectile)
        {
            projectile.Deactivate(_projectileContext);

            LeanPool.Despawn(projectile.gameObject);
        }

        private void LogMessage(string message)
        {
            Debug.Log($"{Runner.name} (tick: {Runner.Tick}, frame: {Time.frameCount}) - {message}");
        }
    }
}
