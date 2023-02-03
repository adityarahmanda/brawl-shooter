using Fusion;
using UnityEngine;

namespace BrawlShooter
{
    [CreateAssetMenu(fileName = "Database", menuName = "Brawl Shooter/Database", order = 1)]
    public class DatabaseData : ScriptableObject
    {
        public CharacterData[] characterDatas;

        public CharacterData GetCharacterData(NetworkString<_64> id)
        {
            foreach (var data in characterDatas)
            {
                if (data.Id == id) return data;
            }

            return null;
        }
    }
}