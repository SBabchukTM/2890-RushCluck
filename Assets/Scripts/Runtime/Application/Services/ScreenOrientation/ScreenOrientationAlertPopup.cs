using Core.UI;
using DG.Tweening;
using UnityEngine;

namespace Core.Services.ScreenOrientation
{
    public class ScreenOrientationAlertPopup : BasePopup
    {
        private const float PortretModeRotationAngleZ = 330f;
        private const float LandscapeModeRotationAngleZ = 240f;
        private const float AnimationInSpeed = 2f;
        private const float AnimationOutSpeed = 0.3f;

        [SerializeField] private RectTransform _phoneRecTransform;

        private Sequence _currentRotationSequence;

        private float _previousTimeScale;

        private void OnEnable()
        {
            _previousTimeScale = Time.timeScale;

            if (Time.timeScale > 0f)
                Time.timeScale = 0f;

            _phoneRecTransform.rotation = Quaternion.Euler(0, 0, LandscapeModeRotationAngleZ);
            StartRotatingPhoneIconAnimation();
        }

        private void OnDisable()
        {
            Time.timeScale = _previousTimeScale;

            _phoneRecTransform.DOKill();
            _currentRotationSequence?.Kill();
        }


        private void StartRotatingPhoneIconAnimation()
        {
            _currentRotationSequence = DOTween.Sequence()
                .Append(_phoneRecTransform.DORotate(
                            new Vector3(0, 0, PortretModeRotationAngleZ),
                            AnimationInSpeed,
                            RotateMode.FastBeyond360)
                        .SetEase(Ease.InOutSine))
                .Append(_phoneRecTransform.DORotate(
                            new Vector3(0, 0, LandscapeModeRotationAngleZ),
                            AnimationOutSpeed,
                            RotateMode.FastBeyond360)
                        .SetEase(Ease.InOutQuad))
                .SetLoops(-1);
        }
    }
}