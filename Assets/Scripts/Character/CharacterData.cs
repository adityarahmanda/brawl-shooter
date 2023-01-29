using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrawlShooter
{
    [CreateAssetMenu(fileName = "Character", menuName = "Brawl Shooter/Character", order = 1)]
    public class CharacterData : ScriptableObject
    {
        public string characterName;
        public Character characterPrefab;
        public PlayerAgent agentPrefab;
    }
}