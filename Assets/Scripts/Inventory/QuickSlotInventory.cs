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

    // Объект у которого дочерние объекты являются слотами
    public Transform quickslotParent;
    public InventoryManager inventoryManager;
    public int currentQuickslotID = 0;
    public Sprite selectedSprite;
    public Sprite notSelectedSprite;
    public Text healthText;

    // Update is called once per frame
    void Update()
    {
        // Используем цифры
        for (int i = 0; i < quickslotParent.childCount; i++)
        {
            // если мы нажимаем на клавиши 1 по 6 то...
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                // проверяем если наш выбранный слот равен слоту который у нас уже выбран, то
                if (currentQuickslotID == i)
                {
                    // Ставим картинку "selected" на слот если он "not selected" или наоборот
                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == notSelectedSprite)
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                    }
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                    }
                }
                // Иначе мы убираем свечение с предыдущего слота и светим слот который мы выбираем
                else
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                    currentQuickslotID = i;
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                }
            }
        }

        // Используем предмет по нажатию на E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item != null)
            {
                if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.isConsumeable &&
                    !inventoryManager.isOpened &&
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == selectedSprite)
                {
                    // Применяем изменения к здоровью
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
        // Получаем HealthSystem с игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Объект с тегом 'Player' не найден!");
            return;
        }

        HealthSystem healthSystem = player.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem не найден на игроке!");
            return;
        }

        // Получаем текущий слот и проверяем предмет
        InventorySlot slot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
        if (slot?.item == null)
        {
            Debug.LogError("Предмет не найден в слоте!");
            return;
        }

        // Применяем изменения здоровья
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