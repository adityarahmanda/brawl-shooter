using Fusion;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace BrawlShooter
{
    public class UILoadingMenu : TweenScreen
    {
        [Header("Loading Settings")]
        [SerializeField]
        private TextMeshProUGUI _loadingText;
        private LoadingType _loadingType;

        [SerializeField]
        private TextMeshProUGUI _ellipsisText;
        private int _ellipsisDotsCount;

        [SerializeField]
        private float _ellipsisAnimationSpeed = .5f;

        private void OnEnable()
        {
            _ellipsisDotsCount = 0;
            StartCoroutine(EllipsisAnimation());
        }

        private void OnDisable()
        {
            StopCoroutine(EllipsisAnimation());
        }

        private IEnumerator EllipsisAnimation()
        {
            while(true)
            {
                if (_ellipsisDotsCount > 3) _ellipsisDotsCount = 0;
                _ellipsisText.text = new string('.', _ellipsisDotsCount);

                yield return new WaitForSeconds(_ellipsisAnimationSpeed);
                
                _ellipsisDotsCount++;
            }
        }

        public void UpdateLoadingText()
        {
            switch (_loadingType)
            {
                case LoadingType.CreateSession:
                    _loadingText.text = "Creating Session";
                    break;
                case LoadingType.LoadScene:
                    _loadingText.text = "Loading Scene";
                    break;
            }
        }

        public void StartLoading(LoadingType type)
        {
            _loadingType = type;

            switch (_loadingType)
            {
                case LoadingType.CreateSession:
                    EventManager.AddEventListener<SessionCreatedEvent>(OnSessionCreated);
                    break;
                case LoadingType.LoadScene:
                    EventManager.AddEventListener<SceneLoadedEvent>(OnSceneLoaded);
                    break;
            }

            UpdateLoadingText();
        }

        public void OnSessionCreated(SessionCreatedEvent e)
        {
            ScreenManager.Instance.HideAll();
            ScreenManager.Instance.Show<UILobbyMenu>().SetRoomName(e.sessionName);
            EventManager.RemoveEventListener<SessionCreatedEvent>(OnSessionCreated);
        }

        public void OnSceneLoaded(SceneLoadedEvent e)
        {
            ScreenManager.Instance.HideAll();
            EventManager.RemoveEventListener<SceneLoadedEvent>(OnSceneLoaded);
        }
    }

    public enum LoadingType
    {
        CreateSession,
        LoadScene
    }
}