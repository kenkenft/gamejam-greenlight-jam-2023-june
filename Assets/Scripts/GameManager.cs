using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [HideInInspector]
    public delegate void OnSomeEvent();
    public static OnSomeEvent RoundHasStarted;
    public static OnSomeEvent CharacterSetUpRequested;


    
    // Start is called before the first frame update
    void Start()
    {
        // SetUp();
        RoundHasStarted?.Invoke();
    }

    void SetUp()
    {
        CharacterSetUpRequested?.Invoke();
    }


}
