using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public SOFightMoves fightMove;

    [HideInInspector] public delegate void FightMoveSent(SOFightMoves fightMove);
    [HideInInspector] public static FightMoveSent FightMoveSelected;
    
    void OnMouseUp()
    {
    //   Debug.Log(this.gameObject.name + " pressed!");
    //  SFXRequested?.Invoke("Click");
        FightMoveSelected?.Invoke(fightMove);
    }

}
