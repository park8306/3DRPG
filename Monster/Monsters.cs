using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monsters : MonoBehaviour
{
    public GameObject[] monsters = new GameObject[6];
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        { 
            monsters[i] = transform.GetChild(i).gameObject;
        }
    }
    Coroutine handle;
    private void Update()
    {
        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i].activeSelf && monsters[i].GetComponent<Monster>().isStartCoroutine == true)
                continue;
            monsters[i].GetComponent<Monster>().isStartCoroutine = true;
            handle = StartCoroutine(MakeMonster(monsters[i]));
        }
    }

    private IEnumerator MakeMonster(GameObject mushroom)
    {
        yield return new WaitForSeconds(6f);
        mushroom.SetActive(true);
        StopCoroutine(handle);
    }
}
