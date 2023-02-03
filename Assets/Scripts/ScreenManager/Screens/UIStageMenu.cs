using Fusion;
using Lean.Pool;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityFx.Outline;
using Unity.VisualScripting;

namespace BrawlShooter
{
    public class UIStageMenu : MonoBehaviour
    {
        [Header("Stage Settings")]
        public Transform CharactersRoot;

        [SerializeField]
        private TextMeshProUGUI _characterNameText;
        private OutlineEffect _outlineEffect;

        private int _selectedIndex;
        private Character[] _characters;

        private void Awake()
        {
            _outlineEffect = Camera.main.GetComponent<OutlineEffect>();
        }

        private void OnEnable()
        {
            EventManager.AddEventListener<PlayerSpawnedEvent>(OnPlayerSpawned);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<PlayerSpawnedEvent>(OnPlayerSpawned);
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            var characterDatas = GlobalSettings.Instance.database.characterDatas;

            _characters = new Character[characterDatas.Length];
            for (int i = 0; i < characterDatas.Length; i++)
            {
                var character = LeanPool.Spawn(characterDatas[i].characterPrefab, CharactersRoot);
                _outlineEffect.AddGameObject(character.gameObject);
                _characters[i] = character;
            }

            _selectedIndex = 0;
            SelectCharacter(_selectedIndex);
        }

        public void ToggleCharacter()
        {
            _selectedIndex++;

            if (_selectedIndex > _characters.Length - 1)
                _selectedIndex = 0;

            SelectCharacter(_selectedIndex);
        }

        public void SelectCharacter(int index)
        {
            if (index < 0 && index > _characters.Length - 1) return;

            for (int i = 0; i < _characters.Length; i++)
            {
                if (i == index)
                {
                    _characterNameText.text = _characters[i].data.characterName;
                }

                _characters[i].gameObject.SetActive(i == index);
            }

            if(NetworkManager.Instance.localPlayer != null)
            {
                NetworkManager.Instance.localPlayer.RPC_SetSelectedCharacterId(_characters[_selectedIndex].data.Id);
            }
        }

        private void OnPlayerSpawned(PlayerSpawnedEvent e)
        {
            if (NetworkManager.Instance.IsLocalPlayer(e.player) == false) return;

            NetworkManager.Instance.localPlayer.RPC_SetSelectedCharacterId(_characters[_selectedIndex].data.Id);
        }
    }
}
