using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BrawlShooter
{
    public class SegmentedProgressBar : ProgressBar
    {
        private bool _isInitialized;

        public int numberOfSegments = 1;
        public float notchSize = 0f;

        [SerializeField]
        private HorizontalLayoutGroup _segmentRoot;

        [SerializeField]
        private ProgressBarSegment _segmentTemplate;

        private List<Image> _progressFill = new List<Image>();

        public bool initializeOnAwake = true;

        public void Awake()
        {
            if (initializeOnAwake) Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            _segmentTemplate.gameObject.SetActive(false);
            _segmentRoot.spacing = notchSize;

            for (int i = 0; i < numberOfSegments; i++)
            {
                ProgressBarSegment currSegment = Instantiate(_segmentTemplate, _segmentRoot.transform);
                currSegment.gameObject.SetActive(true);
                _progressFill.Add(currSegment.fillImage);
            }

            _isInitialized = true;
        }

        public void Update()
        {
            if (!_isInitialized) return;

            for (int i = 0; i < numberOfSegments; i++)
            {
                _progressFill[i].fillAmount = numberOfSegments * fillAmount - i;
            }
        }
    }
}