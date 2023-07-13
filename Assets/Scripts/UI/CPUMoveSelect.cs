using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUMoveSelect : MonoBehaviour
{
    [SerializeField] List<SOFightMoves> AllMoves;
    SOFightMoves[] AllFightPossibleMoves;
    public MatchFlowManager MFM;
    public SOCharacterStats[] Opponents;

    // void Start()
    // {
    //     SetUp();
    // }
    public void SetUp()
    {
        MFM.Fighters[1].SOCS = Opponents[GameProperties.BattleIndex];
        MFM.Fighters[1].SetUp();
        SetUpMovePool();
    }

    void SetUpMovePool()
    {
        AllMoves.Clear();
        foreach(SOFightMoves move in MFM.Fighters[1].SOCS.MovePool)
        {
            AllMoves.Add(move);
        }
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
        string moveID = GameProperties.IDNumToActionID[moveIndex];

        for(int i = 0; i < AllFightPossibleMoves.Length; i++)
        {
            if(AllFightPossibleMoves[i].ID.Equals(moveID))
                return AllFightPossibleMoves[i];
        }

        return null;
    }
}
