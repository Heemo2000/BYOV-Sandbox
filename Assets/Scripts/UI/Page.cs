using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class Page : MonoBehaviour
    {
        [SerializeField]
        private float animationSpeed = 1f;
        public bool exitOnNewPagePush = false;
        [SerializeField]
        private EntryMode entryMode = EntryMode.Slide;
        [SerializeField]
        private Direction entryDirection = Direction.Left;
        [SerializeField]
        private EntryMode exitMode = EntryMode.Slide;
        [SerializeField]
        private Direction exitDirection = Direction.Left;
        public UnityEvent OnPushAction;
        public UnityEvent OnPostPushAction;
        public UnityEvent OnPrePopAction;
        public UnityEvent OnPostPopAction;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Coroutine _animationCoroutine;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Enter(bool playAnimation = true)
        {
            OnPushAction?.Invoke();

            switch(entryMode)
            {
                case EntryMode.Slide:
                    SlideIn(playAnimation);
                    break;
                case EntryMode.Zoom:
                    ZoomIn(playAnimation);
                    break;
                case EntryMode.Fade:
                    FadeIn(playAnimation);
                    break;
            }
        }

        public void Exit(bool playAnimation = true)
        {
	    	OnPrePopAction?.Invoke();

            switch (exitMode)
            {
                case EntryMode.Slide:
                    SlideOut(playAnimation);
                    break;
                case EntryMode.Zoom:
                    ZoomOut(playAnimation);
                    break;
                case EntryMode.Fade:
                    FadeOut(playAnimation);
                    break;
            }
        }

        private void SlideIn(bool playAnimation = true)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.SlideIn(_rectTransform, entryDirection, animationSpeed, OnPostPushAction));

        }

        private void SlideOut(bool playAnimation = true)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.SlideOut(_rectTransform, exitDirection, animationSpeed, OnPostPopAction));

            
        }

        private void ZoomIn(bool playAnimation = true)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.ZoomIn(_rectTransform, animationSpeed, OnPostPushAction));

            
        }

        private void ZoomOut(bool playAnimation = true)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.ZoomOut(_rectTransform, animationSpeed, OnPostPopAction));

            
        }

        private void FadeIn(bool playAnimation = true)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.FadeIn(_canvasGroup, animationSpeed, OnPostPushAction));

            
        }

        private void FadeOut(bool playAnimation = true)
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _animationCoroutine = StartCoroutine(UIAnimationHelper.FadeOut(_canvasGroup, animationSpeed, OnPostPopAction));


        }
    }
}
