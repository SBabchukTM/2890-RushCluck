using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Item", menuName = "Config/Shop Item")]
public class ShopItem : ScriptableObject
{
    [SerializeField] private int _itemID = 0;
    [SerializeField] private int _itemPrice = 0;
    [SerializeField] private Sprite _itemSprite;

    public int ItemID => _itemID;
    public int ItemPrice => _itemPrice;
    public Sprite ItemSprite => _itemSprite;
}
