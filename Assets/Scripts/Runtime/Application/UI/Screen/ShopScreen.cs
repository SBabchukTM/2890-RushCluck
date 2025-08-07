using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ShopScreen : UiScreen
    {
        [SerializeField] private SimpleButton _goBackButton;
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private RectTransform _shopItemsParent;

        public event Action OnBackPressed;

        private void OnDestroy()
        {
            _goBackButton.Button.onClick.RemoveAllListeners();
        }

        public void Initialize()
        {
            SubscribeToEvents();
        }

        public void UpdateBalance(int balance) => _balanceText.text = balance.ToString();

        public void SetShopItems(List<ShopItemDisplay> items)
        {
            foreach (ShopItemDisplay item in items)
                item.transform.SetParent(_shopItemsParent, false);
        }

        private void SubscribeToEvents()
        {
            _goBackButton.Button.onClick.AddListener(() => OnBackPressed?.Invoke());
        }
    }
}