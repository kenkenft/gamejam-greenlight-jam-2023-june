using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private int _maxHP, _currHP;
    public Slider BarSlider;

    void Start()
    {
        UpdateHealthBar(0);
    }
    public void UpdateHealthBar(int amount)
    {
        _currHP -= amount;
        _currHP = Mathf.Clamp(_currHP, 0, _maxHP);
        BarSlider.value = (float)_currHP/ (float)_maxHP;
    } 
}
