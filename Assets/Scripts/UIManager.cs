using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [HideInInspector] 
    public delegate void OnSomeEvent();
    public static OnSomeEvent DisableButtonsRequested;


    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
        ActionButton.MoveSelected += DisableAllMoveButtons; 
        
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
        ActionButton.MoveSelected -= DisableAllMoveButtons; 
    }

    void SetUp()
    {
        Debug.Log("UIManager.SetUp called");
        DisableAllMoveButtons();
    }

    public void DisableAllMoveButtons()
    {
        DisableButtonsRequested?.Invoke();
    }

}
