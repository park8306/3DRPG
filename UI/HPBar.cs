using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    Transform cam;
    Slider hpBarSlider;
    void Start()
    {
        cam = Camera.main.transform;
        hpBarSlider = transform.Find("Slider").GetComponent<Slider>();
    }

    void Update()
    {

        transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
        //transform.LookAt(Camera.main.transform);
    }

    internal void UpdateMonsterHPBar(int hP, int maxHP)
    {
        hpBarSlider.value = (float)hP / maxHP;
    }
}
