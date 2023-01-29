using Fusion;
using Lean.Pool;
using TMPro;
using UnityEngine;

namespace BrawlShooter
{
    public class UIStageMenu : BaseScreen
    {
        [Header("Stage Settings")]
        public CharacterData[] characterDatas;
        public Transform CharactersRoot;

        [SerializeField]
        private TextMeshProUGUI _characterNameText;

        private int _selectedIndex;
        private Character[] _characters;

        public Character SelectedCharacter => _characters != null ? _characters[_selectedIndex] : null;

        private void OnEnable()
        {
            EventManager.AddEventListener<GameStartedEvent>(OnGameStarted);
            EventManager.AddEventListener<PlayerSelectCharacterEvent>(OnPlayerSelectCharacter);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<GameStartedEvent>(OnGameStarted);
            EventManager.RemoveEventListener<PlayerSelectCharacterEvent>(OnPlayerSelectCharacter);
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            _characters = new Character[characterDatas.Length];
            for (int i = 0; i < characterDatas.Length; i++)
            {
                var character = LeanPool.Spawn(characterDatas[i].characterPrefab, CharactersRoot);
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
        }

        public void OnGameStarted(GameStartedEvent e)
        {
            Local.RPC_SetCharacterData(_selectedIndex);
        }

        public void OnPlayerSelectCharacter(PlayerSelectCharacterEvent e)
        {
            e.player.CharacterData = _characters[e.player.selectedCharacterIndex].data;
        }
    }
}
