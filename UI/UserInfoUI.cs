using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoUI : MonoBehaviour
{
    public static UserInfoUI Instance;
    Image HPFill;
    Image MPFill;
    TextMeshProUGUI text_Level;
    TextMeshProUGUI HPText;
    int playerMaxHP;
    private void Awake()
    {
        Instance = this;
        HPFill = transform.Find("UserInfo/Slider_Blue/Fill_Area/HPFill").GetComponent<Image>();
        MPFill = transform.Find("UserInfo/Slider_Yellow/MPFill_Area/MPFill").GetComponent<Image>();
        text_Level = transform.Find("UserInfo/FrameIcon_Level/TextLevel").GetComponent<TextMeshProUGUI>();
        HPText = transform.Find("UserInfo/Slider_Blue/HPText").GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        MPFill.fillAmount = 1;
        ChangeHP(Player.Instance.HP);
        ChangeLevel(Player.Instance.playerLevel);
    }

    private void Update()
    {
        ChangeMP(Player.Instance.MP);
        if(playerMaxHP != Player.Instance.MaxHP)
        {
            ChangeHP(Player.Instance.HP);
        }
    }

    internal void ChangeHP(int playerHP)
    {
        HPFill.fillAmount = (float)playerHP / Player.Instance.MaxHP;
        HPText.text = $"{playerHP} / {Player.Instance.MaxHP}";
        playerMaxHP = Player.Instance.MaxHP;
    }
    internal void ChangeLevel(int playerLevel)
    {
        text_Level.text = playerLevel.ToString();
    }

    internal void ChangeMP(int playerMP)
    {
        MPFill.fillAmount = (float)playerMP / Player.Instance.MaxMP;
    }
}
