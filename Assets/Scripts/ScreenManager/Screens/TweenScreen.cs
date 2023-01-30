using UnityEngine;
using DG.Tweening;

namespace BrawlShooter
{
    public class TweenScreen : BaseScreen 
    {
        private Vector2 _initPosition, _moveFromPos;
        private float screenWidth, screenHeight;

        [Header("Show Animation")]
        public AnimationType showAnimation = AnimationType.Fade;
        public float showDuration = 0.5f;
        public Ease showEase = Ease.OutCirc;

        [Header("Hide Animation")]
        public AnimationType hideAnimation = AnimationType.Fade;
        public float hideDuration = 0.5f;
        public Ease hideEase = Ease.InCirc;

        private void Start()
        {
            if (showAnimation == AnimationType.Fade)
            {
                canvasGroup.alpha = 0f;
            }
            else
            {
                _moveFromPos = GetMoveFromPosition(showAnimation);
                rectTransform.anchoredPosition = _moveFromPos;
            }
        }

        protected Vector2 GetMoveFromPosition(AnimationType animationType)
        {
            screenWidth = Screen.width > 720 ? Screen.width : 720;
            screenHeight = Screen.height > 1280 ? Screen.height : 1280;

            _initPosition = rectTransform.anchoredPosition;

            switch (animationType)
            {
                case AnimationType.TopToBottom:
                    return new Vector2(_initPosition.x, screenHeight);
                case AnimationType.BottomToTop:
                    return new Vector2(_initPosition.x, -screenHeight);
                case AnimationType.LeftToRight:
                    return new Vector2(-screenWidth, _initPosition.y);
                case AnimationType.RightToLeft:
                    return new Vector2(screenWidth, _initPosition.y);
            }

            return Vector2.zero;
        }

        protected Vector2 GetMoveToPosition(AnimationType animationType)
        {
            switch (animationType)
            {
                case AnimationType.TopToBottom:
                    return new Vector2(_initPosition.x, -screenHeight);
                case AnimationType.BottomToTop:
                    return new Vector2(_initPosition.x, screenHeight);
                case AnimationType.LeftToRight:
                    return new Vector2(screenWidth, _initPosition.y);
                case AnimationType.RightToLeft:
                    return new Vector2(-screenWidth, _initPosition.y);
                default:
                    return Vector2.zero;
            }
        }

        protected override void OnAnimationIn()
        {
            if (showAnimation == AnimationType.Fade)
            {
                canvasGroup.DOFade(1f, showDuration).From(0f).SetEase(showEase).OnComplete(base.OnAnimationIn);
            }
            else
            {
                Vector2 fromPos = GetMoveFromPosition(showAnimation);
                transform.DOLocalMove(Vector2.zero, showDuration).From(fromPos).SetEase(showEase).OnComplete(base.OnAnimationIn);
            }
        }

        protected override void OnAnimationOut()
        {
            if (hideAnimation == AnimationType.Fade)
            {
                canvasGroup.DOFade(0f, hideDuration).From(1f).SetEase(hideEase).OnComplete(base.OnAnimationOut);
            }
            else
            {
                Vector2 toPos = GetMoveToPosition(hideAnimation);
                transform.DOLocalMove(toPos, hideDuration).From(Vector2.zero).SetEase(hideEase).OnComplete(base.OnAnimationOut);
            }
        }
    }


}