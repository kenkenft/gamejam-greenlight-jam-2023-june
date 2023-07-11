using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    
    // public GameObject EndScreen;

    [HideInInspector] 
    public static OnSomeEvent DisableButtonsRequested;
    public static SendInt EndConditionsMet;
    public static SendInt EndingNumberPassed;


    void OnEnable()
    {        
        // MatchFlowManager.WhichEndingDetermined += GetEnding;
    }

    void OnDisable()
    {
        // MatchFlowManager.WhichEndingDetermined -= GetEnding;
    }

    void GetEnding(int endingNumber)
    {
        Debug.Log($"End condition: {endingNumber}");
        // EndScreen.SetActive(true);
        // EndingNumberPassed?.Invoke(endingNumber);
    }


}
