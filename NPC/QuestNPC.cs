using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("퀘스트 NPC에 들어옴");
            UIManager.instance.isPlayerQuestIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("퀘스트 NPC에서 나감");
            UIManager.instance.isPlayerQuestIn = false;
        }
    }
}
