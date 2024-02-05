using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ShopItemPanel : MonoBehaviour
{

    public Button EquipTab;
    public Button PotionTab;
    public Button RingTab;

    public Button[] TapMenuButtons = new Button[3];

    public ShopBaseItem baseItem;

    public string buttonName;
    public MySQL.ItemType itemType;

    List<ShopBaseItem> baseItemList = new List<ShopBaseItem>(30);

    public bool isInventoryActive = false;
    private void Awake()
    {
        baseItem = GameObject.Find("ShopBaseItem").GetComponent<ShopBaseItem>();
    }
    void OnEnable()
    {
        EquipTab = transform.Find("TapMenu/TapMenu_Button/EquipTab").GetComponent<Button>();
        PotionTab = transform.Find("TapMenu/TapMenu_Button/PotionTab").GetComponent<Button>();
        RingTab = transform.Find("TapMenu/TapMenu_Button/RingTab").GetComponent<Button>();

        TapMenuButtons[0] = EquipTab;
        TapMenuButtons[1] = PotionTab;
        TapMenuButtons[2] = RingTab;

        foreach (var item in TapMenuButtons)
        {
            item.onClick.AddListener(() => { SelectTapMenu(); });
        }

        baseItem.gameObject.SetActive(false);

        SelectTapMenu();
    }
    private void OnDisable()
    {
        isInventoryActive = false;
    }

    public void SelectTapMenu()
    {
        if (!isInventoryActive)
        {
            buttonName = "EquipTab";
            isInventoryActive = true;
        }
        else
            buttonName = EventSystem.current.currentSelectedGameObject.name;
        Image[] buttonImages;
        // 이미지 밝은걸 다 꺼주는 작업
        foreach (var item in TapMenuButtons)
        {
            buttonImages = item.GetComponentsInChildren<Image>(true);
            for (int i = 1; i < buttonImages.Length; i++)
            {
                if (buttonImages[i].gameObject.name == "IconNormal")
                    continue;
                buttonImages[i].gameObject.SetActive(false);
            }
        }
        // 이제 선택된 오브젝트의 이미지만 빛나도록 하자
        foreach (var item in TapMenuButtons)
        {
            if (buttonName == item.name)
            {
                baseItemList.ForEach(x => Destroy(x.gameObject));
                baseItemList.Clear();
                baseItem.gameObject.SetActive(true);
                var buttonNameImages = item.GetComponentsInChildren<Image>(true);
                for (int i = 1; i < buttonNameImages.Length; i++)
                {
                    buttonNameImages[i].gameObject.SetActive(true);
                }
            }

        }
        if (buttonName == "AllTab" || buttonName == "Purchase_Button")
        {
            for (int i = 0; i < MySQL.instance.allItems.Count; i++)
            {
                var newItem = Instantiate(baseItem, baseItem.transform.parent);
                newItem.Init(UserDB.instance.userInventoryItems[i].userItemInfo);
                baseItemList.Add(newItem);
                baseItemList[i].gameObject.SetActive(true);
            }
        }
        else
        {
            switch (buttonName)
            {
                case "EquipTab":
                    itemType = MySQL.ItemType.Weapon | MySQL.ItemType.Armor | MySQL.ItemType.Shield;

                    break;
                case "PotionTab":
                    itemType = MySQL.ItemType.Potion;
                    break;
                case "RingTab":
                    itemType = MySQL.ItemType.Ring;
                    break;
            }

            var newItemList = (from value in MySQL.instance.allItems.Values
                               where value.itemType == (itemType & value.itemType)
                               select value).ToList();
            for (int i = 0; i < newItemList.Count; i++)
            {
                var newItem = Instantiate(baseItem, baseItem.transform.parent);
                newItem.Init(newItemList[i]);
                baseItemList.Add(newItem);
                baseItemList[i].gameObject.SetActive(true);
            }
        }
        baseItem.gameObject.SetActive(false);
    }
}
