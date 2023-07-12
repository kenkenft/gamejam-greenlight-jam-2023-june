using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenButton : MonoBehaviour
{
    public bool IsReturnTitle;
    private bool _isContinuing = false;
    [HideInInspector]
    public static OnSomeEvent EndScreenSet, MethodToCall;

    void OnEnable()
    {
        EndScreenUI.ButtonMethodDetermined += SubscribeRetryOrContinue;
    }

    void OnDisable()
    {
        EndScreenUI.ButtonMethodDetermined -= SubscribeRetryOrContinue;
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
                EndScreenSet += ContinueGame;
            else
                EndScreenSet += RetryFight;
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
    }

    public void ContinueGame()
    {
        Debug.Log("ContinueGame called");      
    }


    public void OnClickReturnToTitle()
    {
        
    }
}
