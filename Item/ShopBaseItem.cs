using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBaseItem : MonoBehaviour
{

    public Image frame;
    public Image itemIcon;
    public Image nofify;
    public Button Button;

    public MySQL.ItemDB baseItemItemInfo;
    internal void Init(MySQL.ItemDB itemInfo)
    {
        frame = transform.Find("GradeFrame").GetComponent<Image>();
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        nofify = transform.Find("Nofify").GetComponent<Image>();
        Button = transform.GetComponent<Button>();

        baseItemItemInfo = itemInfo;

        itemIcon.sprite = Resources.Load<Sprite>(itemInfo.iconName.ToString());
        Button.onClick.AddListener(() => { ShowItemInfo(itemInfo); });
    }

    private void ShowItemInfo(MySQL.ItemDB itemInfo)
    {
        // 클릭한 아이템 정보를 넘겨줌
        ItemInfoShow.selectedItem = itemInfo;
        // 아이템 보여주는 함수 실행
        ItemInfoShow.instance.ShowItemInfo();
    }
}
