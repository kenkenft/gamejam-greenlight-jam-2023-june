using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider[] HealthSliders;
    public Slider[] EnergySliders;

    void OnEnable()
    {
        CharacterDisplay.HealthAffected += UpdateHealthBar;
        CharacterDisplay.EnergyAffected += UpdateEnergyBar;
    }     

    void OnDisable()
    {
        CharacterDisplay.HealthAffected -= UpdateHealthBar;
        CharacterDisplay.EnergyAffected -= UpdateEnergyBar;
    }     
    
    public void UpdateHealthBar(int[] data)
    {
        if(HealthSliders.Length != 0 && data[0] < HealthSliders.Length)
        {
            HealthSliders[data[0]].value = (float)data[2]/ (float)data[1];
        }

    } 

    public void UpdateEnergyBar(int[] data)
    {
        if(EnergySliders.Length != 0 && data[0] < EnergySliders.Length)
        {
            EnergySliders[data[0]].value = (float)data[2]/ (float)data[1];
        }
    } 
}
