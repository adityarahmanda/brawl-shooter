using UnityEngine;
using UnityEngine.UI;

namespace TowerBomber
{
    public class BulletBar : ProgressBar
    {
        [SerializeField] private Player _player;
        [SerializeField] private RawImage _progressBorderTile;

        protected override void Start()
        {
            base.Start();
            var uvRect = _progressBorderTile.uvRect;
            _progressBorderTile.uvRect = new Rect(uvRect.x, uvRect.y, _player.weapon.magazineSize, uvRect.height);
        }

        protected override void Update()
        {
            base.Update();
            SetProgress(_player.bulletsLeft, _player.weapon.magazineSize);
        }
    }
}