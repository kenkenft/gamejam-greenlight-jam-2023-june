using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenUI : MonoBehaviour
{
    public Canvas EndScreen; 
    public Image[] EndRoundImages, CutsceneBackgrounds;
    public Text EndScreenText;

    [HideInInspector]
    public static SendBool DidPlayerWin;
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
        // Debug.Log($"SetEndScreen called. Ending no. {endingNumber}");
        
        if(endingNumber != 0)
        {
            DidPlayerWin?.Invoke(true);
            EndScreenText.text = $"Humanity\nDefended";
            EndScreenText.color = GameProperties.ColourPalleteRGBA["LightBlue"];
        }
        else
        {
            DidPlayerWin?.Invoke(false);
            EndScreenText.text = $"Humanity\nAnnihilated";
            EndScreenText.color = GameProperties.ColourPalleteRGBA["LightGrey"];
        }
    }


}
