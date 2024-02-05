using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuestBaseItem : MonoBehaviour
{
    public TextMeshProUGUI QuestTitleText;
    public Image QuestClearIcon;
    QuestItems questItems;  // QuestItems.instance를 사용하기 위해 추가
    public MySQL.QuestDB questDB;   // 버튼하나하나에 퀘스트 정보를 추가시켜준다.

    public void Init(MySQL.QuestDB questInfo)
    {
        QuestTitleText = transform.Find("QuestTitleText").GetComponent<TextMeshProUGUI>();
        QuestClearIcon = transform.Find("QuestClearIcon").GetComponent<Image>();
        questDB = questInfo;
        QuestTitleText.text = questDB.questTitle;
        QuestClearIcon.gameObject.SetActive(false);

        transform.GetComponent<Button>().onClick.AddListener(() => { QuestInfoPrint(); });
    }

    private void QuestInfoPrint()
    {
        questItems = QuestItems.instance;
        questItems.selectedQuest = questDB; // 선택된 퀘스트를 알려준다
        Debug.Log(questDB.questID);
        questItems.questInfoText.text = questDB.questInfo;
        questItems.questRewardGoldAmount.text = $"골드 : {questDB.rewardGold.ToNumber()}";
        questItems.questRewardExpAmount.text = questDB.rewardExp.ToNumber();
        // 만약 클릭한 퀘스트가 유저가 받은 퀘스트라면 수락버튼을 띄워주지 않음
        if (MySQL.instance.questDbDictionary[questItems.selectedQuest.questID].isQuestAccept)
            questItems.questAcceptButton.gameObject.SetActive(false);
        else
            questItems.questAcceptButton.gameObject.SetActive(true);
        // 만약 클릭한 퀘스트가 보상 받을 퀘스트면 완료 버튼을 띄워줌
        if(MySQL.instance.questDbDictionary[questItems.selectedQuest.questID].isQuestClear && MySQL.instance.questDbDictionary[questItems.selectedQuest.questID].isQuestReward == false)
            questItems.questClearButton.gameObject.SetActive(true);
        else
            questItems.questClearButton.gameObject.SetActive(false);
    }
}
