using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PurchaseItem : MonoBehaviour
{
    Button purchaseButton;

    ShopItemPanel shopItemPanel;

    private void OnEnable()
    {
        shopItemPanel = GameObject.Find("Shop_Right_Panel").transform.GetComponent<ShopItemPanel>();

        purchaseButton = transform.Find("Purchase_Button").GetComponent<Button>();

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => { PurchaseItems(); });
    }

    private void PurchaseItems()
    {
        UserItem item = new UserItem();
        item.userItemInfo = ItemInfoShow.selectedItem;
        item.userItemCount = 1;
        
        if (UserDB.instance.playerGold < item.userItemInfo.itemPrice)
        {
            Debug.Log("플레이어의 소지금이 부족함");
            return;
        }
        // 플레이어의 소지금을 판매금액만큼 빼고 DB에 값을 바꿔주자
        UserDB.instance.playerGold -= item.userItemInfo.itemPrice;
        MySQL.instance.UpdateUserData();

        // 인벤토리 업데이트, 골드 업데이트 시켜주자
        PlayerMoney.instance.ChangeUserMoney();
        // 포션 종류가 아닌 아이템은 플레이어 소지품에 추가시켜주자
        if (ItemInfoShow.selectedItem.itemType != MySQL.ItemType.Potion)
        {
            UserDB.instance.userInventoryItems.Add(item);
            MySQL.instance.UserItemInsert(item.userItemInfo.itemID);
        }
        else // 포션이면 갯수를 증가시켜주자
        {
            if(UserDB.instance.userInventoryItems.Count == 0)
            {
                UserDB.instance.userInventoryItems.Add(item);
                MySQL.instance.UserItemInsert(item.userItemInfo.itemID);
            }
            else
            {
                try
                {
                    var ItemsOwnedByTheUser = (from value in UserDB.instance.userInventoryItems
                                               where value.userItemInfo.itemID == ItemInfoShow.selectedItem.itemID
                                               select value).First();
                    ItemsOwnedByTheUser.userItemCount++;
                    MySQL.instance.UserItemUpdate(ItemsOwnedByTheUser.userItemInfo.itemID, true);
                    // 퀵슬롯의 포션 갯수도 바꿔줘야 됨
                }
                catch (InvalidOperationException)
                {
                    UserDB.instance.userInventoryItems.Add(item);
                    MySQL.instance.UserItemInsert(item.userItemInfo.itemID);
                }
            }
        }
        Debug.Log($"{ItemInfoShow.selectedItem.itemName}을 구입했습니다");
    }
}