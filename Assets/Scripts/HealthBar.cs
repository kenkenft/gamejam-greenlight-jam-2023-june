using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider[] PlayerSliders;

    void OnEnable()
    {
        CharacterDisplay.HealthAffected += UpdateHealthBar;
    }     

    void OnDisable()
    {
        CharacterDisplay.HealthAffected -= UpdateHealthBar;
    }     
    
    public void UpdateHealthBar(int[] data)
    {
        PlayerSliders[data[0]].value = (float)data[2]/ (float)data[1];
        if(data[1] <= 0)
        {
            Debug.Log("Player " + data[0] + " defeated!");
        }
    } 
}
