using UnityEngine;
using Fusion;

namespace BrawlShooter
{
    public abstract class NetworkSingleton<T> : NetworkBehaviour where T : Component
    {

        #region Fields

        /// <summary>
        /// The instance.
        /// </summary>
        private static T instance;

        [SerializeField]
        private bool _dontDestroyOnLoad = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    
                    if (instance == null)
                    {
                        NetworkLauncher.Instance.Runner.Spawn(new GameObject(), Vector3.zero, Quaternion.identity, null, OnBeforeSpawned);

                        void OnBeforeSpawned(NetworkRunner runner, NetworkObject networkObject)
                        {
                            networkObject.name = typeof(T).Name;
                            instance = networkObject.gameObject.AddComponent<T>();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Use this for initialization.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                if(_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

    }
}