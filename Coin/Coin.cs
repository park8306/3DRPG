using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinPoint;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            UserDB.instance.playerGold += coinPoint;
            PlayerMoney.instance.ChangeUserMoney();
            MySQL.instance.UpdateUserData();
            //Destroy(gameObject);
            gameObject.SetActive(false);
            transform.parent = CoinObjectPool.instance.gameObject.transform;
            CoinObjectPool.instance.coins.Add(gameObject);
        }
    }
}
