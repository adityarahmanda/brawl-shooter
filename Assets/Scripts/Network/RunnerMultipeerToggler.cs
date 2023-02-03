using Fusion;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using Fusion.Editor;
#endif

namespace BrawlShooter
{
    [DisallowMultipleComponent]
    [ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
    public class RunnerMultipeerToggler : Fusion.Behaviour
    {
        private static RunnerMultipeerToggler _instance;

        public void Awake()
        {
            if (NetworkProjectConfig.Global.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
            {
                Debug.LogWarning($"{nameof(ToggleRunnerVisibility)} only works in Multi-Peer mode. Destroying.");
                Destroy(this);
                return;
            }

            // Enforce singleton across all Runners.
            if (_instance)
            {
                Destroy(this);

            }
            _instance = this;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                ToggleAll(-1);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                ToggleAll(0);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                ToggleAll(1);
            }
        }

        private void ToggleAll(int runnerIndex)
        {

            var runners = NetworkRunner.GetInstancesEnumerator();

            int index = 0;
            while (runners.MoveNext())
            {
                var runner = runners.Current;

                // Ignore inactive runners - might just be unused scene objects or other orphans.
                if (runner == null || !runner.IsRunning)
                    continue;

                bool enable = runnerIndex == -1 || index == runnerIndex;
                runner.IsVisible = enable;
                runner.ProvideInput = enable;
                
                var virtualCamera = runner.MultiplePeerUnityScene.FindObjectOfType<CinemachineVirtualCamera>(true);
                if (virtualCamera != null)
                {
                    virtualCamera.gameObject.SetActive(enable);
                }

                var eventSystem = runner.MultiplePeerUnityScene.FindObjectOfType<EventSystem>(true);
                if (eventSystem != null)
                {
                    eventSystem.gameObject.SetActive(false);
                }

                index++;
            }
#if UNITY_EDITOR
            // If we have a RunnerVisiblityControlWindow open, it needs to know to refresh.
            if (RunnerVisibilityControlsWindow.Instance)
            {
                RunnerVisibilityControlsWindow.Instance.Repaint();
            }
#endif
        }
    }
}