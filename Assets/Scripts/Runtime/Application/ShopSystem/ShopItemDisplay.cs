using Runtime.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private GameObject _statusObject;
    [SerializeField] private SimpleButton _purchaseButton;

    private bool _inAnim = false;

    private ShopItem _shopItem;
    public ShopItem ShopItem => _shopItem;

    public event Action<ShopItemDisplay> OnPurchasePressed;

    private void OnDestroy()
    {
        _purchaseButton.Button.onClick.RemoveAllListeners();
    }

    public void Initialize(ShopItem shopItem)
    {
        _shopItem = shopItem;

        _itemImage.sprite = _shopItem.ItemSprite;
        _priceText.text = _shopItem.ItemPrice.ToString();

        _purchaseButton.Button.onClick.AddListener(() => OnPurchasePressed?.Invoke(this));
    }

    public void SetStatus(bool status) => _statusObject.SetActive(status);

    public async UniTaskVoid Shake(CancellationToken token, PurchaseFailedShakeParameters purchaseFailedShakeParameters)
    {
        if (_inAnim)
            return;

        _inAnim = true;
        _purchaseButton.transform.DOShakePosition(purchaseFailedShakeParameters.ShakeDuration,
                                          purchaseFailedShakeParameters.Strength,
                                          purchaseFailedShakeParameters.Vibrato,
                                          purchaseFailedShakeParameters.Randomness,
                                          purchaseFailedShakeParameters.Snapping,
                                          purchaseFailedShakeParameters.FadeOut,
                                          purchaseFailedShakeParameters.ShakeRandomnessMode).SetLink(gameObject);

        await UniTask.WaitForSeconds(purchaseFailedShakeParameters.ShakeDuration);
        token.ThrowIfCancellationRequested();

        _inAnim = false;
    }
}
