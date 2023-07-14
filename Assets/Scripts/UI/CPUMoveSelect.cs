using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUMoveSelect : MonoBehaviour
{
    [SerializeField] List<SOFightMoves> KaijuMovePool;
    SOFightMoves[] AllFightPossibleMoves;
    public MatchFlowManager MFM;
    public SOCharacterStats[] Opponents;
    GameProperties.KaijuPersonalities CurrentBehaviour;
    List<GameProperties.KaijuPersonalities> PresetBehaviours = new List<GameProperties.KaijuPersonalities>(){};

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
        SetUpBehaviours();   // ToDo Set up behaviour
    }

    void SetUpMovePool()
    {
        KaijuMovePool.Clear();
        foreach(SOFightMoves move in MFM.Fighters[1].SOCS.MovePool)
        {
            KaijuMovePool.Add(move);
        }
    }

    void SetUpBehaviours()
    {
        PresetBehaviours.Clear();
        GameProperties.KaijuPersonalities[] behaviours = MFM.Fighters[1].SOCS.Behaviours;
        for(int i = 0; i < behaviours.Length; i++)
            PresetBehaviours.Add(behaviours[i]);
        CurrentBehaviour = PresetBehaviours[0];
        MFM.Fighters[1].CurrentBehaviour = CurrentBehaviour;
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
        List<SOFightMoves> filteredListC = AttacksWithinRange(filteredListB);
        List<SOFightMoves> filteredListD = RemoveEnabledBuffMoves(filteredListC);

        if(filteredListD.Count > 1)     // Kaiju can do something
        {
            
            int[] MoveTypeProbabilityRatioModifiers = CalculateMoveTypeBaseBiases(filteredListD);
            
            CurrentBehaviour = MFM.Fighters[1].CurrentBehaviour; // Debugging line. Comment out when not using inspector
            // CurrentBehaviour = SetNewBehaviour(); // ToDo
            MoveTypeProbabilityRatioModifiers = AdjustRatiosBasedOnFactors(MoveTypeProbabilityRatioModifiers);
            bool canOnlyDoNothing = CheckForNothing(MoveTypeProbabilityRatioModifiers);
            // CPUMove = PickAMove(filteredListD, MoveTypeProbabilityRatioModifiers);
            
            if (CPUMove != null && !canOnlyDoNothing)    
                return CPUMove;
            else
                return KaijuMovePool[0];    //  Assumes index 0 contains SOFightMoves called "Nothing".
        } 
        else
            return KaijuMovePool[0]; // Assume kaiju can only do nothing
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

        int[] kaijuSubSystemsData = GetKaijuSubSystemData();
        foreach(SOFightMoves move in movePool)
        {
            if(!AbleToDoMove(kaijuSubSystemsData, move))
                filteredList.Remove(move);
        }
        // for(int i = 0; i < filteredList.Count; i++)
        //     Debug.Log($"SubSystem requirements met for: {filteredList[i].Name}");

        return filteredList;
    }

    int[] GetKaijuSubSystemData()
    {
        int[] SubSystemsData = {0, 0, 0, 0, 0},
                kaijuHealthData = MFM.Fighters[1].HealthSystemData;

        for(int i = 0; i < SubSystemsData.Length ; i++)
        {
            int index = (i * 2) + 3;
            if(kaijuHealthData[index] > 0)
                SubSystemsData[i] = 1;
        }

        // for(int i = 0; i < SubSystemsData.Length; i++)
        //     Debug.Log("kaijuEnergyAndSubSystemsData[" + i +"]: "+ SubSystemsData[i]);

        return SubSystemsData;
    }

    bool AbleToDoMove(int[] subSystemsData, SOFightMoves move)
    {
        bool isRequirementMet = true;
        
        List<int> mandatoryRequirements = new List<int>(), flexibleRequirements = new List<int>();
        int subSystemIndex, requirementValue;   
        // Parse list into mandatory, and flexible requirement lists
        for(int i = 0 ; i < subSystemsData.Length ; i++)
        {
            subSystemIndex = i + 1; //SOFightMoves.Requirements is a 6 item array, where subsystem related data starts at index 1
            requirementValue = move.Requirements[subSystemIndex];

            if(requirementValue == 1)
                mandatoryRequirements.Add(subSystemIndex);
            else if (requirementValue == 2)
                flexibleRequirements.Add(subSystemIndex);
        }

        // Check for mandatory subsystems i.e. playerEnergyAndSubSystemsData[i] = 1
        isRequirementMet = CheckSubSystemRequirement(subSystemsData, mandatoryRequirements, true);
        
        // Check for requirements where it requires only one of the arms i.e. playerEnergyAndSubSystemsData[i] = 2
        if(isRequirementMet && flexibleRequirements.Count > 0)
            isRequirementMet = CheckSubSystemRequirement(subSystemsData, flexibleRequirements, false);

        return isRequirementMet;
    }   // End of CheckSubSystems

    bool CheckSubSystemRequirement(int[] subSystemsData, List<int> requirements, bool isMandatory)
    {
        if(isMandatory)
        {
            int index;
            for(int i = 0; i < requirements.Count ; i++)
            {
                index = requirements[i] - 1;    // Offset as subSystemData is of array 5 and indices in requirements derives from a 6-size integer
                if(subSystemsData[index] == 0)
                {    
                    // Debug.Log("Mandatory subsytem broken for fightMove: " + FightMove.Name);
                    return false;   // i.e. At least one mandatory subsystem is broken
                }
            }
        }
        else
        {
            int index;
            for(int i = 0; i < requirements.Count ; i++)
            {
                index = requirements[i] - 1;
                if(subSystemsData[index] == 1)
                    return true;    // i.e. At least one of the valid subsystem candidates is available for the flexible requirement
            }
            // Debug.Log("All candidate subsytems broken for fightMove: " + FightMove.Name);
            return false;   // i.e. All valid subsystem candidate that could be used for the flexible requirement are broken
        }
        return true;    // i.e All mandatory subsystems are active
    }   // End of CheckSubSystemRequirement

    List<SOFightMoves> AttacksWithinRange(List<SOFightMoves> movePool)
    {
        List<SOFightMoves> filteredList = new List<SOFightMoves>(movePool);
        int distanceFromPlayer = MFM.Fighters[1].CurrentTileID - MFM.Fighters[0].CurrentTileID;
        bool withinAttacksRange;
        foreach(SOFightMoves move in movePool)
        {
            if(move.ActionType == GameProperties.ActionType.Attack)
            {
                withinAttacksRange = distanceFromPlayer <= move.Range;
                if(!withinAttacksRange)
                    filteredList.Remove(move);  
                // Debug.Log($"Can {move.Name} hit target? {withinAttacksRange}");
            }
        }

        // for(int i = 0; i < filteredList.Count; i++)
        //     Debug.Log($"Range requirement met for: {filteredList[i].Name}");

        return filteredList;
    }

    List<SOFightMoves> RemoveEnabledBuffMoves(List<SOFightMoves> movePool)
    {
        List<SOFightMoves> filteredList = new List<SOFightMoves>(movePool);
        int activeBuff = MFM.Fighters[1].GetActiveHeadBuff();
        // Debug.Log($"Active buff: {(GameProperties.BuffTypes)activeBuff}");
        foreach(SOFightMoves move in movePool)
        {
            if(move.ActionType == GameProperties.ActionType.Special && move.MainEffectValue[0] == activeBuff)
            {
                filteredList.Remove(move);
                // Debug.Log($"Remove buff move: {move.Name}");
            }
        }
        return filteredList;
    }
    
    int[] CalculateMoveTypeBaseBiases(List<SOFightMoves> movePool)
    {
        GameProperties.ActionType[] buffTypes = (GameProperties.ActionType[])System.Enum.GetValues(typeof(GameProperties.ActionType));
        int[] probabilityRatios = new int [buffTypes.Length]; 

        // Set ratios to 0
        for(int i = 0; i < probabilityRatios.Length; i++)
            probabilityRatios[i] = 0;
        
        // Give non-zero positive value to the corresponding action type ratio IF a single move of that type is found.
        foreach(GameProperties.ActionType actionType in buffTypes)
        {
            foreach(SOFightMoves move in movePool)
            {
                if(actionType == move.ActionType)
                {
                    probabilityRatios[ (int)actionType ] += 100;
                    break;
                }
            }
        }
        
        // for(int i = 0; i < probabilityRatios.Length; i++)
        //     Debug.Log($"{(GameProperties.ActionType)i} ratio value: {probabilityRatios[i]}");
        return probabilityRatios;
    }

    int[] AdjustRatiosBasedOnFactors(int[] probabilityRatios)
    {
        int[] behaviourModifiers = GameProperties.PersonalitiyModifier[CurrentBehaviour];
        for(int i = 0; i < probabilityRatios.Length; i++)
        {
            if(probabilityRatios[i] != 0)
            {
                probabilityRatios[i] += behaviourModifiers[i];
                probabilityRatios[i] = Mathf.Clamp(probabilityRatios[i], 0, 1000);
            }
        }

        for(int i = 0; i < probabilityRatios.Length; i++)
            Debug.Log($"{(GameProperties.ActionType)i} modified ratio value: {probabilityRatios[i]}");
        
        return probabilityRatios;
    }

    bool CheckForNothing(int[] probabilityModifiers)
    {
        for(int i = 0; i < probabilityModifiers.Length; i++)
        {
            if(probabilityModifiers[i] > 0)
                return false;
        }
        //if all probabilityModifiers are 0, then the only move that the kaiju can do is Nothing.
        return true;
    }
}
