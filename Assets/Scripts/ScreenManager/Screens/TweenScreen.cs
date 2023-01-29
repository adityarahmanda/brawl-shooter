using UnityEngine;

namespace BrawlShooter
{
    public class TweenScreen : BaseScreen 
    {
        private Vector2 _initPosition, _moveFromPos;
        private float screenWidth, screenHeight;

        [Header("Show Animation")]
        public AnimationType showAnimation = AnimationType.Fade;
        public float showDuration = 0.5f;
        public LeanTweenType showEase = LeanTweenType.easeOutCirc;

        [Header("Hide Animation")]
        public AnimationType hideAnimation = AnimationType.Fade;
        public float hideDuration = 0.5f;
        public LeanTweenType hideEase = LeanTweenType.easeInCirc;

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
                LeanTween.alphaCanvas(canvasGroup, 1f, showDuration)
                         .setFrom(0f).setEase(showEase)
                         .setOnComplete(base.OnAnimationIn);
            }
            else
            {
                Vector2 fromPos = GetMoveFromPosition(showAnimation);

                LeanTween.moveLocal(this.transform.gameObject, Vector2.zero, showDuration)
                         .setFrom(fromPos).setEase(showEase)
                         .setOnComplete(base.OnAnimationIn);
            }
        }

        protected override void OnAnimationOut()
        {
            if (hideAnimation == AnimationType.Fade)
            {
                LeanTween.alphaCanvas(canvasGroup, 0f, hideDuration)
                         .setFrom(1f).setEase(hideEase)
                         .setOnComplete(base.OnAnimationOut);
            }
            else
            {
                Vector2 toPos = GetMoveToPosition(hideAnimation);

                LeanTween.moveLocal(this.transform.gameObject, toPos, hideDuration)
                         .setFrom(Vector2.zero).setEase(hideEase)
                         .setOnComplete(base.OnAnimationOut);
            }
        }
    }


}