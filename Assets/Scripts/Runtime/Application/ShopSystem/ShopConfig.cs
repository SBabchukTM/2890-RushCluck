using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemsConfig", menuName = "Config/ShopItemsConfig")]
public class ShopConfig : ScriptableObject
{
    [SerializeField] private List<ShopItem> _shopItems = new();
    [SerializeField] private PurchaseEffectSettings _purchaseEffectSettings;

    public List<ShopItem> ShopItems => _shopItems;
    public PurchaseEffectSettings PurchaseEffectSettings => _purchaseEffectSettings;

    private void OnValidate()
    {
        HashSet<int> uniqueIDs = new();

        foreach (ShopItem item in _shopItems)
        {
            if (!uniqueIDs.Add(item.ItemID))
                Debug.LogError($"Shop item {item.name} does not have a unique ID!");
        }
    }
}

[Serializable]
public class PurchaseEffectSettings
{
    public bool PlaySoundOnPurchase = true;
    public bool PlaySoundOnSelectPurchased = true;
    public bool PlaySoundOnNotEnoughCurrency = true;
    public bool ShakeIfNotEnoughCurrency = true;
    public PurchaseFailedShakeParameters PurchaseFailedShakeParameters;
}

[Serializable]
public class PurchaseFailedShakeParameters
{
    [Min(0.01f)] public float ShakeDuration = 0.2f;
    [Min(0.01f)] public float Strength = 20f;
    public int Vibrato = 100;
    [Min(0.01f)] public float Randomness = 90f;
    public bool Snapping = false;
    public bool FadeOut = true;
    public ShakeRandomnessMode ShakeRandomnessMode = ShakeRandomnessMode.Harmonic;
}
