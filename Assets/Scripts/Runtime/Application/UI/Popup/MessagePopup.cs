using System.Threading;
using Core.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class MessagePopup : BasePopup
    {
        [SerializeField] private SimpleButton _closeButton;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _closeButton.Button.onClick.AddListener(DestroyPopup);
            return base.Show(data, cancellationToken);
        }
    }
}