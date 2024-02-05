using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoShow : MonoBehaviour
{
    public static ItemInfoShow instance;

    public TextMeshProUGUI text_Type;
    public TextMeshProUGUI text_ItemName;
    public TextMeshProUGUI text_Stat;
    public TextMeshProUGUI text_ItemExplain;
    public TextMeshProUGUI text_Price;
    public Image item;
    public GameObject selectedItemGO;
    public PurchaseItem purchaseItem;

    private void Awake()
    {
        instance = this;
    }
    public static MySQL.ItemDB selectedItem;
    private void OnEnable()
    {
        text_Type = transform.Find("Text_Type").GetComponent<TextMeshProUGUI>();
        text_ItemName = transform.Find("SelectedItem/ItemName").GetComponent<TextMeshProUGUI>();
        text_ItemExplain = transform.Find("SelectedItem/Info/Text_ItemExplain").GetComponent<TextMeshProUGUI>();
        text_Stat = transform.Find("SelectedItem/Info/Text_Info/Text_Stat").GetComponent<TextMeshProUGUI>();
        item = transform.Find("SelectedItem/GradeFrame/Item").GetComponent<Image>();
        selectedItemGO = transform.Find("SelectedItem").GetComponent<GameObject>();
        text_Price = transform.Find("SelectedItem/Price/Text_Info/Text_Price").GetComponent<TextMeshProUGUI>();
        purchaseItem = transform.Find("SelectedItem/SelectPurchase").GetComponent<PurchaseItem>();
    }
    private void Update()
    {
        if (selectedItem != null)
            purchaseItem.gameObject.SetActive(true);
        else
        {
            purchaseItem.gameObject.SetActive(false);
        }

    }
    internal void ShowItemInfo()
    {
        item.color = Color.white;
        item.sprite = Resources.Load<Sprite>(selectedItem.iconName);
        text_Type.text = selectedItem.itemType.ToString();
        text_ItemName.text = selectedItem.itemName;
        text_ItemExplain.text = selectedItem.itemExplain;
        text_Stat.text = selectedItem.itemAmount.ToString();
        text_Price.text = $"{selectedItem.itemPrice.ToNumber()} Gold";
    }

    private void OnDisable()
    {
        item.color = Color.black;
        text_Type.text = null;
        text_ItemName.text = null;
        text_ItemExplain.text = null;
        text_Stat.text = null;
        selectedItem = null;
    }
}
