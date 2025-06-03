using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public enum ItemType { Default, Food, Weapon, Instrument }
[Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemScriptableObject : ScriptableObject
{

    public string itemName;
    public int maximumAmount;
    public GameObject itemPrefab;
    public Sprite icon;
    public ItemType itemType;
    public string itemDescription;
    public bool isConsumeable;

    [Header("Consumable Characteristics")]
    public int changeHealth;
}
