using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinObjectPool : MonoBehaviour
{
    public static CoinObjectPool instance;
    private void Awake()
    {
        instance = this;
    }

    public List<GameObject> coins = new List<GameObject>();
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            coins.Add((GameObject)Instantiate(Resources.Load("Coin"), transform));
            coins[i].name = $"coin{i}";
            coins[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (coins.Count == 0)
        {
            var newcoin = (GameObject)Instantiate(Resources.Load("Coin"), transform);
            newcoin.SetActive(false);
            coins.Add(newcoin);
        }
    }
}
