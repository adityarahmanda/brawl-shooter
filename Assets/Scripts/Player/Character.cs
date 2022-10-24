using UnityEngine;

namespace BrawlShooter
{
    public class Character : MonoBehaviour
    {
        private Player _player;

        public void SetPlayer(Player player)
        {
            _player = player;
        }

        public void Shoot()
        {
            if (_player == null)
                return;

            _player.Shoot();
        }

        public void CheckEndShoot()
        {
            if (_player == null)
                return;

            _player.CheckEndShoot();
        }
    }
}