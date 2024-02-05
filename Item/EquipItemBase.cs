using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipItemBase : MonoBehaviour, IPointerClickHandler
{
    public Image Icon;
    public MySQL.ItemType equipItemItemType;
    public UserItem equipItemInfo;
    public UserItem initEquipItemInfo;
    MySQL.ItemType firstItemIcon;
    Color firstColor;
    public void Init(MySQL.ItemType itemType)
    {
        Icon = transform.Find("Icon").GetComponent<Image>();
        equipItemItemType = itemType;
        initEquipItemInfo = equipItemInfo;
        //equipItemInfo.user_item_info = new MySQL.ItemDB();
        //equipItemInfo.user_item_info.itemID = 0;
        Icon.sprite = Resources.Load<Sprite>($"icon_{itemType}");
        firstColor = Icon.color;
        firstItemIcon = itemType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if (equipItemInfo.userItemInfo.itemID == 0)
                return;
            Icon.sprite = Resources.Load<Sprite>($"icon_{firstItemIcon}");
            Icon.SetNativeSize();
            Icon.color = firstColor;
            UserDB.instance.userInventoryItems.Add(equipItemInfo);
            UserDB.instance.Unequip(equipItemInfo);
            equipItemInfo = initEquipItemInfo;
            // 해제 했을 때 인벤토리가 해제한 아이템을 보여줘야됨
            UserItemPanel.instance.buttonName = "AllTab";
            UserItemPanel.instance.SelectTapMenu();
        }
    }
}
