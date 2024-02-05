using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkillCoolTime : MonoBehaviour
{
    public CanvasGroup skillIconCanvas;
    public Image skillCountBg;
    public CanvasGroup skillCountBgCanvas;
    public TextMeshProUGUI skillCount;
    void Start()
    {
        skillIconCanvas = transform.Find("SkillIcon").GetComponent<CanvasGroup>();
        skillCountBg = transform.Find("SkillCountBg").GetComponent<Image>();
        skillCountBgCanvas = transform.Find("SkillCountBg").GetComponent<CanvasGroup>();
        skillCount = transform.Find("SkillCount").GetComponent<TextMeshProUGUI>();

        skillCountBg.gameObject.SetActive(false);
        skillCount.gameObject.SetActive(false);
    }

    internal void SkillCoolTimeActive()
    {
        skillIconCanvas.alpha = 0.4f;
        skillCountBg.gameObject.SetActive(true);
        skillCount.gameObject.SetActive(true);
        StartCoroutine(SkillCoolTimeActiveCo());
    }

    private IEnumerator SkillCoolTimeActiveCo()
    {
        while (Player.Instance.skillAbleTime < 5)
        {
            skillCount.text =(Math.Round(5 - Player.Instance.skillAbleTime,1)).ToString();
            skillCountBg.GetComponent<Image>().fillAmount = Player.Instance.skillAbleTime / 5;
            yield return null;
        }
        skillIconCanvas.alpha = 1f;
        skillCountBg.gameObject.SetActive(false);
        skillCount.gameObject.SetActive(false);
    }
}
