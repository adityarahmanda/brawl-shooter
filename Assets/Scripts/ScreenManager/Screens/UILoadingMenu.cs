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

        private int _ellipsisDotsCount;
        private string _ellipsisText;

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
                _ellipsisText = new string('.', _ellipsisDotsCount);
                UpdateLoadingText();

                yield return new WaitForSeconds(_ellipsisAnimationSpeed);
                
                _ellipsisDotsCount++;
            }
        }

        public void UpdateLoadingText()
        {
            switch (_loadingType)
            {
                case LoadingType.CreateSession:
                    _loadingText.text = "Creating Session" + _ellipsisText;
                    break;
                case LoadingType.StartGame:
                    _loadingText.text = "Starting Game" + _ellipsisText;
                    break;
                case LoadingType.LoadScene:
                    _loadingText.text = "Loading Scene" + _ellipsisText;
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
                case LoadingType.StartGame:
                    if (Runner.IsServer)
                    {
                        StartCoroutine(StartingGame());
                    }
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

        public IEnumerator StartingGame()
        {
            yield return new WaitUntil(WaitAllPlayersHasSelectedCharacter);

            StartLoading(LoadingType.LoadScene);
            Launcher.LoadGameplay();
        }

        public bool WaitAllPlayersHasSelectedCharacter()
        {
            foreach (var player in Players)
            {
                if (!player.HasSelectedCharacter) return false;
            }

            return true;
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
        StartGame,
        LoadScene
    }
}