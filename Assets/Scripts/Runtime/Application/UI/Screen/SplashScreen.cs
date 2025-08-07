using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class SplashScreen : UiScreen
    {
        [SerializeField] private float _loadTime;
        [SerializeField] private Slider _slider;

        public override async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            await base.ShowAsync(cancellationToken);
        }

        public override async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            await WaitSplashScreenAnimationFinish(cancellationToken);
            await base.HideAsync(cancellationToken);
        }

        private async UniTask WaitSplashScreenAnimationFinish(CancellationToken cancellationToken)
        {
            _slider.value = 0;
            _slider.DOValue(1, _loadTime);
            await UniTask.Delay((int)(_loadTime * 1000), cancellationToken: cancellationToken);
        }
    }
}