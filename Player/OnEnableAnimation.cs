using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnEnableAnimation : MonoBehaviour
{
    Animator animator;
    TextMeshProUGUI LevelText;
    Transform cam;
    private void OnEnable()
    {
        cam = Camera.main.transform;
        animator = GetComponent<Animator>();
        LevelText = transform.Find("LevelUpBadge/LevelText").GetComponent<TextMeshProUGUI>();
        if (Player.Instance == null)
            return;
        else
            LevelText.text = (Player.Instance.playerLevel + 1).ToString();
        StartCoroutine(OnEnableAnim());
    }

    private void Update()
    {
        transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
    }
    private IEnumerator OnEnableAnim()
    {
        animator.Play("LevelUpAnimation");
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
