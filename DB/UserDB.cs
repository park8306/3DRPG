using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDB : MonoBehaviour
{
    static public UserDB instance;

    public string userId;
    public string userName;
    public int playerGold = 0;
    public int playerGem = 0;
    public int userLv = 0;
    public int userExp = 0;

    
    private void Awake()
    {
        instance = this;
    }

    public List<UserItem> userInventoryItems = new List<UserItem>(24);
    public UserEquip userEquipItem = new UserEquip();

    // 유저가 받은 퀘스트 목록
    public List<MySQL.QuestDB> userAcceptQuests = new List<MySQL.QuestDB>(10);

    public void EquipMounting(UserItem baseItemInfo)
    {
        switch (baseItemInfo.userItemInfo.itemType)
        {
            case MySQL.ItemType.Weapon:
                userEquipItem.userEquipWeapon = baseItemInfo.userItemInfo;
                Player.Instance.playerDamage += userEquipItem.userEquipWeapon.itemAmount;
                break;
            case MySQL.ItemType.Armor:
                userEquipItem.userEquipArmor = baseItemInfo.userItemInfo;
                Player.Instance.MaxHP += baseItemInfo.userItemInfo.itemAmount;
                UserInfoUI.Instance.ChangeHP(Player.Instance.HP);
                break;
            case MySQL.ItemType.Shield:
                userEquipItem.userEquipShield = baseItemInfo.userItemInfo;
                Player.Instance.playerArmor += baseItemInfo.userItemInfo.itemAmount;
                break;
            case MySQL.ItemType.Ring:
                userEquipItem.userEquipRing = baseItemInfo.userItemInfo;
                Player.Instance.playerDamage += baseItemInfo.userItemInfo.itemAmount;
                break;
            case MySQL.ItemType.Potion:
                break;
        }
    }

    internal void Unequip(UserItem equipItemInfo)
    {
        switch (equipItemInfo.userItemInfo.itemType)
        {
            case MySQL.ItemType.Weapon:
                Player.Instance.playerDamage -= userEquipItem.userEquipWeapon.itemAmount;
                userEquipItem.userEquipWeapon = null ;
                break;
            case MySQL.ItemType.Armor:
                Player.Instance.MaxHP -= equipItemInfo.userItemInfo.itemAmount;
                userEquipItem.userEquipArmor = null;
                UserInfoUI.Instance.ChangeHP(Player.Instance.HP);
                break;
            case MySQL.ItemType.Shield:
                Player.Instance.playerArmor -= equipItemInfo.userItemInfo.itemAmount;
                userEquipItem.userEquipShield = null;
                break;
            case MySQL.ItemType.Ring:
                Player.Instance.playerDamage -= equipItemInfo.userItemInfo.itemAmount;
                userEquipItem.userEquipRing = null;
                break;
            case MySQL.ItemType.Potion:
                break;
        }
    }
}
[System.Serializable]
public class UserItem
{
    public MySQL.ItemDB userItemInfo; // db에서 불러온 아이템 정보, 아이템 정보
    public int userItemCount; // 아이템 갯수
}
public class UserEquip
{
    public MySQL.ItemDB userEquipWeapon;
    public MySQL.ItemDB userEquipArmor;
    public MySQL.ItemDB userEquipShield;
    public MySQL.ItemDB userEquipRing;
}