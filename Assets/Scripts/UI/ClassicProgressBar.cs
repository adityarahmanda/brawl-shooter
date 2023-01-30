using UnityEngine;
using UnityEngine.UI;

namespace BrawlShooter
{
    public class ClassicProgressBar : ProgressBar
    {
        [SerializeField]
        private Image _fillImage;

        public void Update()
        {
           _fillImage.fillAmount = fillAmount;
        }
    }
}