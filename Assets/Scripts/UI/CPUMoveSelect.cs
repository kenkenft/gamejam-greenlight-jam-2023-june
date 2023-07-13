using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUMoveSelect : MonoBehaviour
{
    [SerializeField] List<SOFightMoves> KaijuMovePool;
    SOFightMoves[] AllFightPossibleMoves;
    public MatchFlowManager MFM;
    public SOCharacterStats[] Opponents;

    // void Start()
    // {
    //     SetUp();
    // }

    void OnEnable()
    {
        MatchFlowManager.CPUMoveRequested += SelectMove;
    }

    void OnDisable()
    {
        MatchFlowManager.CPUMoveRequested -= SelectMove;
    }

    public void SetUp()
    {
        MFM.Fighters[1].SOCS = Opponents[GameProperties.BattleIndex];
        MFM.Fighters[1].SetUp();
        SetUpMovePool();
    }

    void SetUpMovePool()
    {
        KaijuMovePool.Clear();
        foreach(SOFightMoves move in MFM.Fighters[1].SOCS.MovePool)
        {
            KaijuMovePool.Add(move);
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
                KaijuMovePool.Add( moveToAdd );
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

    SOFightMoves SelectMove()
    {
        SOFightMoves CPUMove = null;
        List<SOFightMoves> potentialMoves = KaijuMovePool;
        // Determine which moves are valid given the kaiju's state
        // List<SOFightMoves> filteredListA = HaveEnoughEnergy(KaijuMovePool);
        // List<SOFightMoves> filteredListB = WithinRange(filteredListA);
        // List<SOFightMoves> filteredListC = AvailableSubSystems(filteredListB);
        // CPUMove = PickAMove(filteredListC);
        return CPUMove; 
    }
}
