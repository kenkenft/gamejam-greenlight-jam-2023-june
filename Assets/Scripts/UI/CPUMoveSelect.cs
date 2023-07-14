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
        List<SOFightMoves> potentialMoves = new List<SOFightMoves>(KaijuMovePool);
        // Determine which moves are valid given the kaiju's state
        List<SOFightMoves> filteredListA = HaveEnoughEnergy(potentialMoves);
        // List<SOFightMoves> filteredListB = AvailableSubSystems(filteredListA);
        // List<SOFightMoves> filteredListC = WithinRange(filteredListB);
        // int[] TypeProbabilityRatioModifiers = CalculateMoveTypeBiases(filteredListC);
        // CPUMove = PickAMove(filteredListC);
        return filteredListA[0];
        // return CPUMove; 
    }

    List<SOFightMoves> HaveEnoughEnergy(List<SOFightMoves> movePool)
    {
        int currEnergy = MFM.Fighters[1].EnergyData[0];
        List<SOFightMoves> filteredList = new List<SOFightMoves>(movePool);
        foreach(SOFightMoves move in movePool)
        {
            if(move.Requirements[0] > currEnergy)
                filteredList.Remove(move);
        }

        for(int i = 0; i < filteredList.Count; i++)
            Debug.Log($"Enough energy for: {filteredList[i].Name}");
        return filteredList;
    }
}
