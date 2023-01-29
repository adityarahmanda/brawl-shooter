using UnityEngine;
using UnityEngine.UI;

namespace BrawlShooter
{
    public class BulletBar : ProgressBar
    {
        [SerializeField] 
        private RawImage _progressBorderTile;

        public void SetMaxBullets(int maxBullets)
        {
            var uvRect = _progressBorderTile.uvRect;
            _progressBorderTile.uvRect = new Rect(uvRect.x, uvRect.y, maxBullets, uvRect.height);
        }

    }
}