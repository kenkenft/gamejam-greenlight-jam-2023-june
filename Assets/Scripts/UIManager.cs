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
        ActionButton.MoveSelected += DisableAllMoveButtons; 
    }

    void OnDisable()
    {
        ActionButton.MoveSelected -= DisableAllMoveButtons; 
    }

    public void DisableAllMoveButtons()
    {
        DisableButtonsRequested?.Invoke();
    }

}
