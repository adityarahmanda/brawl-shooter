using UnityEngine;

namespace BrawlShooter
{
    public abstract class ProgressBar : MonoBehaviour
    {
        [Range(0, 1f)]
        public float fillAmount = 1f;
    }
}