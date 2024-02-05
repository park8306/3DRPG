using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuestItems : MonoBehaviour
{
    public static QuestItems instance;
    QuestBaseItem questBaseItem;
    List<QuestBaseItem> questBaseItems = new List<QuestBaseItem>();
    public MySQL.QuestDB selectedQuest;    // 최근 선택된 퀘스트가 무엇인지 알아보기위함
    public TextMeshProUGUI questInfoText;
    public TextMeshProUGUI questRewardGoldAmount;
    public TextMeshProUGUI questRewardExpAmount;
    public Button questAcceptButton;
    public Button questClearButton;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        questBaseItem = transform.Find("Quest_Title/QuestTitles/Scroll View/Viewport/Content/QuestBaseItem").GetComponent<QuestBaseItem>();
        questInfoText = transform.Find("Quest_Info/ScrollRect/Contents/QuestInfoText").GetComponent<TextMeshProUGUI>();
        questRewardGoldAmount = transform.Find("Quest_Info/QuestReward/QuestRewardItems/QuestRewardGold/QuestRewardGoldAmount").GetComponent<TextMeshProUGUI>();
        questRewardExpAmount = transform.Find("Quest_Info/QuestReward/QuestRewardItems/QuestRewardExp/QuestRewardExpAmount").GetComponent<TextMeshProUGUI>();
        questAcceptButton = transform.Find("Quest_Info/AcceptButton").GetComponent<Button>();
        questClearButton = transform.Find("Quest_Info/ClearButton").GetComponent<Button>();


        for (int i = 0; i < MySQL.instance.questDbDictionary.Count; i++)
        {
            var newItem = Instantiate(questBaseItem, questBaseItem.transform.parent);
            newItem.Init(MySQL.instance.questDbDictionary[MySQL.instance.questDBs[i].questID]);
            questBaseItems.Add(newItem);
        }
        questBaseItem.gameObject.SetActive(false);

        questAcceptButton.onClick.RemoveAllListeners();
        questClearButton.onClick.RemoveAllListeners();
        questAcceptButton.onClick.AddListener(() => { QuestAccept(); });
        questClearButton.onClick.AddListener(() => { QuestClear(); });
        questAcceptButton.gameObject.SetActive(false);
        questClearButton.gameObject.SetActive(false);
    }

    private void QuestClear()
    {
        UserDB.instance.playerGold += MySQL.instance.questDbDictionary[selectedQuest.questID].rewardGold;
        UserDB.instance.userExp += MySQL.instance.questDbDictionary[selectedQuest.questID].rewardExp;
        MySQL.instance.questDbDictionary[selectedQuest.questID].isQuestReward = true;
        MySQL.instance.UpdateUserData();
        PlayerMoney.instance.ChangeUserMoney();
        questClearButton.gameObject.SetActive(false);
        for (int i = 0; i < questBaseItems.Count; i++)
        {
            if (questBaseItems[i].questDB.isQuestReward)
            {
                questBaseItems[i].QuestClearIcon.gameObject.SetActive(true);
                for (int j = 0; j < UserDB.instance.userAcceptQuests.Count; j++)
                {
                    if(questBaseItems[i].questDB.questID == UserDB.instance.userAcceptQuests[j].questID)
                    {
                        UserDB.instance.userAcceptQuests.Remove(UserDB.instance.userAcceptQuests[j]);
                    }
                }
            }
        }
        QuickQuestUI.instance.QuickQuestShow();
    }

    private void QuestAccept()
    {
        //selectedQuest를 유저db에 추가시켜줘야됨
        // 선택한 퀘스트의 isQuestAccept가 true면 유저db에 추가시켜주지 않음
        try
        {
            if (MySQL.instance.questDbDictionary[selectedQuest.questID].isQuestAccept)
            {
                return;
            }
            // 유저db의 userAcceptQuests에 선택된 퀘스트를 추가시켜주고 퀘스트db에서는 isQuestAccept를 true로 변경시켜줌
            questAcceptButton.gameObject.SetActive(false);
            UserDB.instance.userAcceptQuests.Add(selectedQuest);
            MySQL.instance.questDbDictionary[selectedQuest.questID].isQuestAccept = true;

            QuickQuestUI.instance.QuickQuestShow();
        }
        catch (KeyNotFoundException kne)
        {
            Debug.Log(kne);
            Debug.Log("퀘스트를 선택해달라는 UI 발생!");
        }
    }
    private void OnDisable()
    {
        selectedQuest = null; // 선택된 퀘스트를 해제한다.
        questInfoText.text = "퀘스트를 선택해주세요!";
        questRewardGoldAmount.text = "골드 : 0";
        questRewardExpAmount.text = "0";
    }
}
