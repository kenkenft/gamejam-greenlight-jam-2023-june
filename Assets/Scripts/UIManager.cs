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
    }

    void OnDisable()
    {
    }




}
