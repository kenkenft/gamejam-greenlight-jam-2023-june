using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{
    public int TimeLeft = 10;
    public Text TimerText;

    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
        MatchFlowManager.TurnHasEnded += HasRoundFinished;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
        MatchFlowManager.TurnHasEnded -= HasRoundFinished;
    }

    void SetUp()
    {
        SetTimer(15);
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
        if(TimeLeft > 1)
            TimerText.text = $"Turns Left\n{TimeLeft}";
        else if(TimeLeft == 1)
            TimerText.text = $"Final turn";
        else
            TimerText.text = $"Time Up!";
    }

    bool HasRoundFinished()
    {
        DecreaseTimer();
        if(TimeLeft > 0)
            return false;
        else 
            return true;
    }
}
