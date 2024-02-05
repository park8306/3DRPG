using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject panel;
    Transform inventoryPanelParent;
    Transform shopPanel;
    Transform questPanel;
    public UserItemPanel itemPanel;
    bool isInventoryUIEnable;
    public bool isShopUIEnable;
    public bool isQuestUIEnable;
    public bool isPlayerShopIn;
    public bool isPlayerQuestIn;

    public Button InventoryCloseButton;
    public Button ShopCloseButton;
    public Button QuestCloseButton;

    //public 

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inventoryPanelParent = transform.Find("Panel_Equipment");
        shopPanel = transform.Find("Shop_Panel");
        questPanel = transform.Find("Quest_Panel");
        isInventoryUIEnable = false;
        isShopUIEnable = false;
        isQuestUIEnable = false;
        isPlayerShopIn = false;
        isPlayerQuestIn = false;
        InventoryCloseButton = transform.Find("Panel_Equipment/Equipment/TopBar/Button_Close").GetComponent<Button>();
        ShopCloseButton = transform.Find("Shop_Panel/TopBar/Button_Close").GetComponent<Button>();
        QuestCloseButton = transform.Find("Quest_Panel/TopBar/Button_Close").GetComponent<Button>();
        InventoryCloseButton.onClick.AddListener(() => { InventoryClose(); });
        ShopCloseButton.onClick.AddListener(() => { ShopUIOpenOrClose(); });
        QuestCloseButton.onClick.AddListener(() => { QuestUIOpenOrClose(); });
    }

    private void InventoryClose()
    {
        InventoryUIOpenOrClose();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryUIOpenOrClose();
        }

        if (Input.GetKeyDown(KeyCode.G) && isPlayerShopIn)
        {
            Debug.Log("상점 오픈!");
            ShopUIOpenOrClose();
        }
        if (Input.GetKeyDown(KeyCode.G) && isPlayerQuestIn)
        {
            Debug.Log("퀘스트 창 오픈!");
            QuestUIOpenOrClose();
        }
    }

    private void InventoryUIOpenOrClose()
    {
        if (isInventoryUIEnable)
        {
            GameManager.gameState = GameState.Play;
            inventoryPanelParent.gameObject.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isInventoryUIEnable = !isInventoryUIEnable;
        }
        else
        {
            GameManager.gameState = GameState.UI;
            inventoryPanelParent.transform.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isInventoryUIEnable = !isInventoryUIEnable;
        }
    }

    public void ShopUIOpenOrClose()
    {
        if (isShopUIEnable)
        {
            GameManager.gameState = GameState.Play;
            shopPanel.gameObject.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isShopUIEnable = !isShopUIEnable;
        }
        else
        {
            GameManager.gameState = GameState.UI;
            shopPanel.transform.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isShopUIEnable = !isShopUIEnable;
        }
    }
    public void QuestUIOpenOrClose()
    {
        if (isQuestUIEnable)
        {
            GameManager.gameState = GameState.Play;
            questPanel.gameObject.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isQuestUIEnable = !isQuestUIEnable;
        }
        else
        {
            GameManager.gameState = GameState.UI;
            questPanel.transform.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isQuestUIEnable = !isQuestUIEnable;
        }
    }
}
