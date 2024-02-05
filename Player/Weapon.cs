using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            other.GetComponent<Monster>().TakeHit(Player.Instance.playerDamage);
        }
    }

    internal void ColliderOn()
    {
        StartCoroutine(ColliderOnCo());
    }
    public float weaponColliderOnTime = 0.2f;
    private IEnumerator ColliderOnCo()
    {
        yield return new WaitForSeconds(weaponColliderOnTime);
        transform.GetComponent<Collider>().enabled = true;
    }
}
