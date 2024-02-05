using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuickQuestUI : MonoBehaviour
{
    static public QuickQuestUI instance;
    QuickQuestBaseItem quickQuestBaseItem;
    public List<QuickQuestBaseItem> quickQuestBaseItems = new List<QuickQuestBaseItem>();

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        quickQuestBaseItem = transform.Find("QuickQuestUIBG/QuickQuestBaseItem").GetComponent<QuickQuestBaseItem>();
    }

    public void QuickQuestShow()
    {
        try
        {
            quickQuestBaseItems.ForEach(x => Destroy(x.gameObject));
            quickQuestBaseItems.Clear();
            quickQuestBaseItem.gameObject.SetActive(true);
            for (int i = 0; i < UserDB.instance.userAcceptQuests.Count; i++)
            {
                var newQuickQuest = Instantiate(quickQuestBaseItem, quickQuestBaseItem.transform.parent);
                newQuickQuest.Init(UserDB.instance.userAcceptQuests[i]);
                quickQuestBaseItems.Add(newQuickQuest);
            }
            quickQuestBaseItem.gameObject.SetActive(false);
        }
        catch (NullReferenceException ex)
        {
            for (int i = 0; i < UserDB.instance.userAcceptQuests.Count; i++)
            {
                var newQuickQuest = Instantiate(quickQuestBaseItem, quickQuestBaseItem.transform.parent);
                newQuickQuest.Init(UserDB.instance.userAcceptQuests[i]);
                quickQuestBaseItems.Add(newQuickQuest);
            }
            quickQuestBaseItem.gameObject.SetActive(false);
            Debug.Log(ex);
        }
    }
}
