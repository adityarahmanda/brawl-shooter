using Fusion;
using TMPro;
using UnityEngine;

namespace BrawlShooter
{
    public class UIRoomMenu : TweenScreen
    {
        [Header("Rooom Settings")]
        public GameMode gameMode;

        [SerializeField]
        private TMP_InputField roomInput;

        public void CreateSession()
        {
            NetworkLauncher.Instance.Launch(gameMode, roomInput.text);
            ScreenManager.Instance.Show<UILoadingMenu>().StartLoading(LoadingType.CreateSession);
        }
    }
}