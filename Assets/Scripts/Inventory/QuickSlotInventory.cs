using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotInventory : MonoBehaviour
{
    private void Awake()
    {
        if (quickslotParent == null)
        {
            Debug.LogError("quickslotParent is not assigned in QuickSlotInventory!");
            quickslotParent = transform;
        }
    }

    // ������ � �������� �������� ������� �������� �������
    public Transform quickslotParent;
    public InventoryManager inventoryManager;
    public int currentQuickslotID = 0;
    public Sprite selectedSprite;
    public Sprite notSelectedSprite;
    public Text healthText;

    // Update is called once per frame
    void Update()
    {
        // ���������� �����
        for (int i = 0; i < quickslotParent.childCount; i++)
        {
            // ���� �� �������� �� ������� 1 �� 6 ��...
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                // ��������� ���� ��� ��������� ���� ����� ����� ������� � ��� ��� ������, ��
                if (currentQuickslotID == i)
                {
                    // ������ �������� "selected" �� ���� ���� �� "not selected" ��� ��������
                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == notSelectedSprite)
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                    }
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                    }
                }
                // ����� �� ������� �������� � ����������� ����� � ������ ���� ������� �� ��������
                else
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                    currentQuickslotID = i;
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                }
            }
        }

        // ���������� ������� �� ������� �� E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item != null)
            {
                if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.isConsumeable &&
                    !inventoryManager.isOpened &&
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == selectedSprite)
                {
                    // ��������� ��������� � ��������
                    ChangeCharacteristics();

                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount <= 1)
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponentInChildren<DragAndDropItem>().NullifySlotData();
                    }
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount--;
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().itemAmountText.text =
                            quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount.ToString();
                    }
                }
            }
        }
    }

    private void ChangeCharacteristics()
    {
        // �������� HealthSystem � ������ �� ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("������ � ����� 'Player' �� ������!");
            return;
        }

        HealthSystem healthSystem = player.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem �� ������ �� ������!");
            return;
        }

        // �������� ������� ���� � ��������� �������
        InventorySlot slot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
        if (slot?.item == null)
        {
            Debug.LogError("������� �� ������ � �����!");
            return;
        }

        // ��������� ��������� ��������
        int healthChange = Mathf.RoundToInt(slot.item.changeHealth);
        if (healthChange > 0)
        {
            healthSystem.Heal(healthChange);
        }
        else if (healthChange < 0)
        {
            healthSystem.TakeDamage(-healthChange);
        }
    }

    public void UpdateQuickSlotsHighlight()
    {
        for (int i = 0; i < quickslotParent.childCount; i++)
        {
            Image slotImage = quickslotParent.GetChild(i).GetComponent<Image>();
            slotImage.sprite = i == currentQuickslotID ? selectedSprite : notSelectedSprite;
        }
    }
}