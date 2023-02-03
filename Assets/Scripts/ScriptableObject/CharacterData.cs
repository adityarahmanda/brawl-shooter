using UnityEngine;

namespace BrawlShooter
{
    [CreateAssetMenu(fileName = "Character", menuName = "Brawl Shooter/Character", order = 1)]
    public class CharacterData : ScriptableObjectId
    {
        public string characterName;
        public Character characterPrefab;
        public PlayerAgent agentPrefab;
    }
}