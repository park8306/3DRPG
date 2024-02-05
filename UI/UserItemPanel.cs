using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class UserItemPanel : MonoBehaviour 
{
    static public UserItemPanel instance;

    public Button AllTab;
    public Button EquipTab;
    public Button PotionTab;
    public Button RingTab;

    public Button[] TapMenuButtons = new Button[4];

    public BaseItem baseItem;
    public List<BaseItem> baseItemList = new List<BaseItem>(30);

    public MySQL.ItemType itemType;
    public string buttonName;
    private void Awake()
    {
        instance = this;
        baseItem = GameObject.Find("ItemBase").GetComponent<BaseItem>();
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        AllTab = transform.Find("TapMenu/TapMenu_Button/AllTab").GetComponent<Button>();
        EquipTab = transform.Find("TapMenu/TapMenu_Button/EquipTab").GetComponent<Button>();
        PotionTab = transform.Find("TapMenu/TapMenu_Button/PotionTab").GetComponent<Button>();
        RingTab = transform.Find("TapMenu/TapMenu_Button/RingTab").GetComponent<Button>();

        TapMenuButtons[0] = AllTab;
        TapMenuButtons[1] = EquipTab;
        TapMenuButtons[2] = PotionTab;
        TapMenuButtons[3] = RingTab;

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
    public bool isInventoryActive = false;
    public void SelectTapMenu()
    {
        if(!isInventoryActive)
        {
            buttonName = "AllTab";
            isInventoryActive = true;
        }
        else
            try
            {
                buttonName = EventSystem.current.currentSelectedGameObject.name;
            }
            catch (NullReferenceException ex)
            {
                Debug.Log($"버튼 이름 예외 발생! {ex}");
                buttonName = "AllTab";
            }
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
            if(buttonName == item.name)
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
        if (buttonName == "AllTab")
        {
            AllTabCreate();
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

            var newItemList = (from value in UserDB.instance.userInventoryItems
                               where value.userItemInfo.itemType == (itemType & value.userItemInfo.itemType)
                               select value).ToList();
            for (int i = 0; i < newItemList.Count; i++)
            {
                var newItem = Instantiate(baseItem, baseItem.transform.parent);
                newItem.Init(newItemList[i].userItemInfo, newItemList[i].userItemCount);
                baseItemList.Add(newItem);
                baseItemList[i].gameObject.SetActive(true);
            }
        }
        baseItem.gameObject.SetActive(false);
    }

    private void AllTabCreate()
    {
        for (int i = 0; i < UserDB.instance.userInventoryItems.Count; i++)
        {
            var newItem = Instantiate(baseItem, baseItem.transform.parent);
            newItem.Init(UserDB.instance.userInventoryItems[i].userItemInfo, UserDB.instance.userInventoryItems[i].userItemCount);
            baseItemList.Add(newItem);
            baseItemList[i].gameObject.SetActive(true);
        }
    }
}
