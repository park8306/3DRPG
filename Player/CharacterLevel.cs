using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterLevel : MonoBehaviour
{
    public static CharacterLevel instance;

    Slider expSlider;
    TextMeshProUGUI level;
    TextMeshProUGUI expText;

    int userExp;
    float userMaxExp;
    float userExpPercent;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        expSlider = transform.Find("Slider").GetComponent<Slider>();
        level = transform.Find("TextLevelNum").GetComponent<TextMeshProUGUI>();
        expText = transform.Find("Slider/TextExp").GetComponent<TextMeshProUGUI>();
        userExp = UserDB.instance.userExp;

        ChangeExp();
        level.text = UserDB.instance.userLv.ToString();
    }
    private void Update()
    {
        if (userExp != UserDB.instance.userExp)
        {
            ChangeCharacterLevelOrExp();
            userExp = UserDB.instance.userExp;
        }
    }
    public void ChangeCharacterLevelOrExp()
    {
        if(UserDB.instance.userExp >= MySQL.instance.LvDBs[UserDB.instance.userLv - 1].maxExp)
        {
            UserDB.instance.userExp -= MySQL.instance.LvDBs[UserDB.instance.userLv - 1].maxExp;
            UserDB.instance.userLv += 1;
            level.text = UserDB.instance.userLv.ToString();
            ChangeExp();
            Player.Instance.playerDamage += 10;
            Player.Instance.playerArmor += 5;
            Player.Instance.MaxHP += 50;
            Player.Instance.LevelUpUIGO.SetActive(true);
            MySQL.instance.UpdateUserData();
        }
        else
        {
            ChangeExp();
            MySQL.instance.UpdateUserData();
        }
    }

    private void ChangeExp()
    {
        userExpPercent = ((float)UserDB.instance.userExp / MySQL.instance.LvDBs[UserDB.instance.userLv - 1].maxExp) * 100;
        expSlider.value = userExpPercent; // 임시 todo : 데이터베이스에 레벨 당 경험치량 지정
        expText.text = userExpPercent.ToString();
    }
}
