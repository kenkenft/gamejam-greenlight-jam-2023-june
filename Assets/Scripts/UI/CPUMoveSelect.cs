using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUMoveSelect : MonoBehaviour
{
    [SerializeField] List<SOFightMoves> AllMoves;
    SOFightMoves[] AllFightPossibleMoves;

    void Start()
    {
        SetUp();
    }
    void SetUp()
    {
        AllMoves.Clear();
        AllFightPossibleMoves = Resources.LoadAll<SOFightMoves>("FightMoves");
        GetMoveSet(GameProperties.BattleIndex);
    }

    void GetMoveSet(int battleIndex)
    {
        int[] moveIDs = GameProperties.KaijuMoveSetIDs[battleIndex];
        
        SOFightMoves moveToAdd = null;
        
        for(int i = 0; i < moveIDs.Length; i++)
        {    
            moveToAdd = MoveIndexToSOFM( moveIDs[i] );
            if(moveToAdd != null) 
                AllMoves.Add( moveToAdd );
            else
                Debug.Log($"Error: moveID Mech{moveIDs[i]} is invalid. Did not add");
        }
    }

    SOFightMoves MoveIndexToSOFM(int moveIndex)
    {
        string moveID = GameProperties.IDNumToActionName[moveIndex];

        for(int i = 0; i < AllFightPossibleMoves.Length; i++)
        {
            if(AllFightPossibleMoves[i].ID.Equals(moveID))
                return AllFightPossibleMoves[i];
        }

        return null;
    }
}
