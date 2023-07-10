using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{
    public int TimeLeft = 10;
    public Text TimerText;

    void Start()
    {
        UpdateTimerText();
    }
    void SetTimer(int amount)
    {
        TimeLeft = amount;
        UpdateTimerText();
    }

    void DecreaseTimer()
    {
        TimeLeft--;
        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        TimerText.text = $"Turns Left\n{TimeLeft}"; 
    }
}
