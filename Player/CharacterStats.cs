using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStats : MonoBehaviour
{
    public static CharacterStats instance;
    TextMeshProUGUI HP;
    TextMeshProUGUI attack;
    TextMeshProUGUI armor;

    Slider hpSlider;
    Slider attackSlider;
    Slider armorSlider;

    int statHp;
    int statMaxHp;
    int statAttack;
    int statArmor;
    int maxStat = 200;

    private void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        HP = transform.Find("Stat_Health/Text").GetComponent<TextMeshProUGUI>();
        attack = transform.Find("Stat_Attack/Text").GetComponent<TextMeshProUGUI>();
        armor = transform.Find("Stat_Armor/Text").GetComponent<TextMeshProUGUI>();

        hpSlider = transform.Find("Stat_Health/Slider").GetComponent<Slider>();
        attackSlider = transform.Find("Stat_Attack/Slider").GetComponent<Slider>();
        armorSlider = transform.Find("Stat_Armor/Slider").GetComponent<Slider>();

        statHp = Player.Instance.HP;
        statMaxHp = Player.Instance.MaxHP;
        statAttack = Player.Instance.playerDamage;
        statArmor = Player.Instance.playerArmor;

        HP.text = $"{statHp}/{statMaxHp}";
        attack.text = statAttack.ToString();
        armor.text = statArmor.ToString();

        hpSlider.value = (float)statHp / statMaxHp;
        attackSlider.value = (float)statAttack / maxStat;
        armorSlider.value = (float)statArmor / maxStat;
    }
    // 플레이어의 스탯이 바뀌는 상황 : 장비를 장착할 때, 해제할 때, 레벨업 할 때
    // 플레이어의 스탯이 바뀌면 스탯정보도 바뀌어주자
    private void Update()
    {
        if(statHp != Player.Instance.HP)
        {
            HP.text = $"{Player.Instance.HP}/{statMaxHp}";
            statHp = Player.Instance.HP;
            hpSlider.value = (float)statHp / statMaxHp;

        }
        if (statMaxHp != Player.Instance.MaxHP)
        {
            HP.text = $"{statHp}/{Player.Instance.MaxHP}";
            statMaxHp = Player.Instance.MaxHP;
            hpSlider.value = (float)statHp / statMaxHp;
        }
        if (statAttack != Player.Instance.playerDamage)
        {
            attack.text = Player.Instance.playerDamage.ToString();
            statAttack = Player.Instance.playerDamage;
            attackSlider.value = (float)statAttack / maxStat;
        }
        if (statArmor != Player.Instance.playerArmor)
        {
            armor.text = Player.Instance.playerArmor.ToString();
            statArmor = Player.Instance.playerArmor;
            armorSlider.value = (float)statArmor / maxStat;
        }
    }
}
