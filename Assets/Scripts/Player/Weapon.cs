using UnityEngine;

namespace TowerBomber
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
    public class Weapon : ScriptableObject
    {
        public enum Type
        {
            SingleShot,
            RapidShot
        };

        [SerializeField]
        public Type type;

        [Range(0, 30f)]
        public float range = 10f;

        [Range(0, 10)]
        public int magazineSize = 3;

        [Range(0, 10f)]
        public float bulletsCooldownTime = .3f;

        [Range(0, 1f)]
        public float timeBetweenShoot = .3f;
    }
}