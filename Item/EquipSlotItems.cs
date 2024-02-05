using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class EquipSlotItems : MonoBehaviour
{
    static public EquipSlotItems instance;

    EquipItemBase equipItemBase;

    public List<EquipItemBase> equipItemBases = new List<EquipItemBase>();

    MySQL.ItemType[] itemTypes = { MySQL.ItemType.Weapon, MySQL.ItemType.Armor, MySQL.ItemType.Shield, MySQL.ItemType.Ring };

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        equipItemBase = transform.Find("EquipItemBase").GetComponent<EquipItemBase>();
        for (int i = 0; i < 4; i++)
        {
            var newEquipItem = Instantiate(equipItemBase, equipItemBase.transform.parent);
            newEquipItem.Init(itemTypes[i]);
            equipItemBases.Add(newEquipItem);
        }
        equipItemBase.gameObject.SetActive(false);
    }

    internal void equipMounting(UserItem baseItemInfo)
    {
        EquipItemBase item = ItemSelect(baseItemInfo);
        item.equipItemInfo = baseItemInfo;
        item.Icon.sprite = Resources.Load<Sprite>(baseItemInfo.userItemInfo.iconName);
        item.Icon.SetNativeSize();
        item.Icon.color = Color.white;
    }

    public EquipItemBase ItemSelect(UserItem baseItemInfo)
    {
        return (from value in equipItemBases
                where value.equipItemItemType == baseItemInfo.userItemInfo.itemType
                select value).First();
    }
}
