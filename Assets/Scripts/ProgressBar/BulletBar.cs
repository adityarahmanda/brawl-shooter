using UnityEngine;
using UnityEngine.UI;

namespace TowerBomber
{
    public class BulletBar : ProgressBar
    {
        [SerializeField] private RawImage _progressBorderTile;
        [SerializeField] private WeaponController _weaponController;

        protected override void Start()
        {
            base.Start();
            var uvRect = _progressBorderTile.uvRect;
            _progressBorderTile.uvRect = new Rect(uvRect.x, uvRect.y, _weaponController.magazineSize, uvRect.height);
        }

        protected override void Update()
        {
            base.Update();
            SetProgress(_weaponController.bulletsLeft, _weaponController.magazineSize);
        }
    }
}