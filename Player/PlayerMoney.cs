using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    static public PlayerMoney instance;
    private void Awake()
    {
        instance = this;
    }
    public TextMeshProUGUI gemText;
    public TextMeshProUGUI goldText;

    // Start is called before the first frame update
    void Start()
    {
        ChangeUserMoney();
    }
    public void ChangeUserMoney()
    {
        gemText = transform.Find("StatusGem/GemText").GetComponent<TextMeshProUGUI>();
        goldText = transform.Find("StatusGold/GoldText").GetComponent<TextMeshProUGUI>();

        gemText.text = UserDB.instance.playerGem.ToNumber();
        goldText.text = UserDB.instance.playerGold.ToNumber();
    }
    
}
