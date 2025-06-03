using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int savedDay;
    public TimeOfDay savedTime;
    public int playerHealth;

    public InventorySaveData inventory;
}

[Serializable]
public class QuestData
{
    public string questID;
    public string status;
}

[Serializable]
public class InventorySaveData
{
    public List<SlotSaveData> slots = new List<SlotSaveData>();
    public List<SlotSaveData> quickSlots = new List<SlotSaveData>();
    public int currentQuickslotID;
}

[Serializable]
public class SlotSaveData
{
    public string itemName;          // ��� ��������
    public int amount;              // ����������
    public bool isEmpty;            // ������ �� ����
    public ItemType itemType;       // ��� ��������
    public bool isConsumeable;      // ����� �� ������������
    public int changeHealth;
}