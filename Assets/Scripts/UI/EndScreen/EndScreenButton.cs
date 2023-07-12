using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenButton : MonoBehaviour
{
    public bool IsReturnTitle;
    private bool _isContinuing = false;

    public Text ButtonText;
    [HideInInspector]
    public static OnSomeEvent EndScreenSet, MethodToCall;

    void OnEnable()
    {
        EndScreenUI.DidPlayerWin += SubscribeRetryOrContinue;
    }

    void OnDisable()
    {
        EndScreenUI.DidPlayerWin -= SubscribeRetryOrContinue;
    }
    
    public void OnClickRetryOrContinue()
    {
        EndScreenSet?.Invoke();
        UnsubscribeRetryOrContinue();
    }

    public void SubscribeRetryOrContinue(bool isContinuing)
    {
        _isContinuing = isContinuing;
        Debug.Log("SubscribeRetryOrContinue called");
        if(!IsReturnTitle)
        {
            if(_isContinuing)
            {    
                EndScreenSet += ContinueGame;
                ButtonText.text = $"Continue";
            }
            else
            {
                EndScreenSet += RetryFight;
                ButtonText.text = $"Retry?";
            }
        }
        else
        {
            ButtonText.text = $"Return to title";
        }
    }

    public void UnsubscribeRetryOrContinue()
    {  
        Debug.Log("UnsubscribeRetryOrContinue called");
        if(!IsReturnTitle)
        {
            // Is there a generic way to remove all possible subscriptions from a delegate?
            if(_isContinuing)
                EndScreenSet -= ContinueGame;
            else
                EndScreenSet -= RetryFight;
        }
        
    }

    public void RetryFight()
    {
        Debug.Log("RetryFight called");
        // ToDo Restart current fight
    }

    public void ContinueGame()
    {
        Debug.Log("ContinueGame called");   
        // ToDo Advance game state   
        // ToDo Set game state
        // ToDo Setup next round
    }


    public void ReturnToTitle()
    {
        Debug.Log("ReturnToTitle called");   
        // ToDo Title screen object
    }
}
