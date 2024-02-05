using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreNPC : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("상점 NPC에 들어옴");
            UIManager.instance.isPlayerShopIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("상점 NPC에서 나감");
            UIManager.instance.isPlayerShopIn = false;
        }
    }
}
