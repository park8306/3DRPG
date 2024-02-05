using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class QuickQuestBaseItem : MonoBehaviour
{
    public MySQL.QuestDB quickQuestBaseItemQuestDB;
    public TextMeshProUGUI quickQuestTitleText;
    public TextMeshProUGUI quickQuestInfoText;
    public TextMeshProUGUI quickQuestGoalText;
    public Image quickQuestClearIcon;

    public void Init(MySQL.QuestDB questDB)
    {
        quickQuestTitleText = transform.Find("QuickQuestTitleBG/QuickQuestTitleText").GetComponent<TextMeshProUGUI>();
        quickQuestInfoText = transform.Find("QuickQuestInfoBG/QuickQuestInfoText").GetComponent<TextMeshProUGUI>();
        quickQuestGoalText = transform.Find("QuickQuestInfoBG/QuickQuestGoalText").GetComponent<TextMeshProUGUI>();
        quickQuestClearIcon = transform.Find("QuickQuestInfoBG/QuickQuestGoalText/QuickQuestClearIcon").GetComponent<Image>();
        
        quickQuestBaseItemQuestDB = questDB;
        if (quickQuestBaseItemQuestDB.isQuestClear)
        {
            quickQuestClearIcon.gameObject.SetActive(true);
        }
        quickQuestTitleText.text = questDB.questTitle;
        quickQuestInfoText.text = questDB.questInfo;
        quickQuestGoalText.text = $"{questDB.questAmount} : {questDB.questTotalGoal}";
    }

    internal void UpdateQuestGoal(MySQL.QuestDB questDB)
    {
        quickQuestGoalText.text = $"{questDB.questAmount} : {questDB.questTotalGoal}";
        if(questDB.isQuestClear)
        {
            quickQuestClearIcon.gameObject.SetActive(true);
        }
            
    }
}
