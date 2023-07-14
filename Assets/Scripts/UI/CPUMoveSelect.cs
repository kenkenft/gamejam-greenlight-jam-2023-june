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
        List<SOFightMoves> filteredListB = AvailableSubSystems(filteredListA);
        // List<SOFightMoves> filteredListC = WithinRange(filteredListB);
        // List<SOFightMoves> filteredListD = WhichBuffEnabled(filteredListC);

        // int[] MoveTypeProbabilityRatioModifiers = CalculateMoveTypeBiases(filteredListD);
        // CPUMove = PickAMove(filteredListD, MoveTypeProbabilityRatioModifiers);
        return filteredListB[0];
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

        // for(int i = 0; i < filteredList.Count; i++)
        //     Debug.Log($"Enough energy for: {filteredList[i].Name}");
        return filteredList;
    }

    List<SOFightMoves> AvailableSubSystems(List<SOFightMoves> movePool)
    {
        List<SOFightMoves> filteredList = new List<SOFightMoves>(movePool);

        int[] kaijuEnergyAndSubSystemsData = GetKaijuEnergyAndSusbSystemData();
        

        return filteredList;
    }

    int[] GetKaijuEnergyAndSusbSystemData()
    {
        int[] kaijuEnergyAndSubSystemsData = {0, 0, 0, 0, 0, 0},
                kaijuHealthData = MFM.Fighters[1].HealthSystemData;

        kaijuEnergyAndSubSystemsData[0] = MFM.Fighters[1].EnergyData[0];

        for(int i = 1; i < kaijuEnergyAndSubSystemsData.Length ; i++)
        {
            int index = (i * 2) + 1 ;
            if(kaijuHealthData[index] > 0)
                kaijuEnergyAndSubSystemsData[i] = 1;
        }

        for(int i = 0; i < kaijuEnergyAndSubSystemsData.Length; i++)
            Debug.Log("kaijuEnergyAndSubSystemsData[" + i +"]: "+ kaijuEnergyAndSubSystemsData[i]);


        return kaijuEnergyAndSubSystemsData;
    }

    // void AbleToDoMove(int[] playerEnergyAndSubSystemsData)
    // {
    //     // playerEnergyAndSubSystemsData // index 0 is the amount of energy available; indexes 1 through 5 are truthy values (0 is broken; 1 is not broken)
    //     bool hasEnoughEnergy = true;
    //     // Check if enough energy is available
    //     if(playerEnergyAndSubSystemsData[0] < FightMove.Requirements[0])   
    //         hasEnoughEnergy = false;

    //     //Check if required subsystems are still active
    //     AreRequirementsMet = CheckSubSystems(playerEnergyAndSubSystemsData, hasEnoughEnergy);
        
    //     //For Buff/Special moves, check whether corresponding buff is already active before enabling button interactable to true 
    //     if(FightMove.ActionType == GameProperties.ActionType.Special)
    //         FightMoveButton.interactable = !CurrentBuffRequested.Invoke(FightMove.MainEffectValue[0]);
    //     else    
    //         FightMoveButton.interactable = AreRequirementsMet;
    // }

    // bool CheckSubSystems(int[] playerEnergyAndSubSystemsData, bool isRequirementMet)
    // {
    //     if(isRequirementMet)
    //     {
    //         List<int> mandatoryRequirements = new List<int>(), flexibleRequirements = new List<int>();
    //         // Parse list into mandatory and flexible
    //         for(int i = 1 ; i < playerEnergyAndSubSystemsData.Length ; i++)
    //         {
    //             // int index = i - 1;
    //             switch(FightMove.Requirements[i])
    //             {
    //                 case 1:
    //                 {
    //                     mandatoryRequirements.Add(i);
    //                     break;
    //                 }
    //                 case 2:
    //                 {
    //                     flexibleRequirements.Add(i);
    //                     break;
    //                 }
    //                 default:
    //                     break;
    //             }
    //         }

    //         // Check for mandatory subsystems i.e. playerEnergyAndSubSystemsData[i] = 1
    //         isRequirementMet = CheckSubSystemRequirement(playerEnergyAndSubSystemsData, mandatoryRequirements, true);
            
    //         // Check for requirements where it requires only one of the arms i.e. playerEnergyAndSubSystemsData[i] = 2
    //         if(isRequirementMet && flexibleRequirements.Count > 0)
    //             isRequirementMet = CheckSubSystemRequirement(playerEnergyAndSubSystemsData, flexibleRequirements, false);
    //     } 

    //     return isRequirementMet;
    // }   // End of CheckSubSystems

    // bool CheckSubSystemRequirement(int[] playerEnergyAndSubSystemsData, List<int> requirements, bool isMandatory)
    // {
    //     if(isMandatory)
    //     {
    //         for(int i = 0; i < requirements.Count ; i++)
    //         {
    //             int index = requirements[i];
    //             if(playerEnergyAndSubSystemsData[index] == 0)
    //             {    
    //                 // Debug.Log("Mandatory subsytem broken for fightMove: " + FightMove.Name);
    //                 return false;   // i.e. At least one mandatory subsystem is broken
    //             }
    //         }
    //     }
    //     else
    //     {
    //         for(int i = 0; i < requirements.Count ; i++)
    //         {
    //             int index = requirements[i];
    //             if(playerEnergyAndSubSystemsData[index] == 1)
    //                 return true;    // i.e. At least one of the valid subsystem candidates is available for the flexible requirement
    //         }
    //         // Debug.Log("All candidate subsytems broken for fightMove: " + FightMove.Name);
    //         return false;   // i.e. All valid subsystem candidate that could be used for the flexible requirement are broken
    //     }
    //     return true;    // i.e All mandatory subsystems are active
    // }   // End of CheckSubSystemRequirement
}
