using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenUI : MonoBehaviour
{
    public Canvas EndScreen; 
    public Image[] EndRoundImages, CutsceneBackgrounds;

    [HideInInspector]
    public static SendBool ButtonMethodDetermined;
    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
        MatchFlowManager.WhichEndingDetermined += SetEndScreen;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
        MatchFlowManager.WhichEndingDetermined -= SetEndScreen;
    }

    void SetUp()
    {
        EndScreen.enabled = false;
    }

    void SetEndScreen(int endingNumber)
    {
        // Set endgame canvas based on loss/win conditions
        EndScreen.enabled = true;
        Debug.Log($"SetEndScreen called. Ending no. {endingNumber}");
        
        if(endingNumber != 0)
        {
            ButtonMethodDetermined?.Invoke(true);
        }
        else
        {
            ButtonMethodDetermined?.Invoke(false);
        }
    }


}
