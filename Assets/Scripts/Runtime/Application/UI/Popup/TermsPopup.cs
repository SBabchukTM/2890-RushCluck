using Runtime.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class TermsPopup : BasePopup
    {
        [SerializeField] private SimpleButton _closeButton;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _closeButton.Button.onClick.AddListener(DestroyPopup);
            return base.Show(data, cancellationToken);
        }
    }
}