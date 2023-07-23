using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUMoveSelect : MonoBehaviour
{
    [SerializeField] List<SOFightMoves> KaijuMovePool;
    SOFightMoves[] AllFightPossibleMoves;
    public MatchFlowManager MFM;
    public SOCharacterStats[] Opponents;
    public SOFightMoves CPUMove;
    GameProperties.KaijuPersonalities CurrentBehaviour;
    List<GameProperties.KaijuPersonalities> PresetBehaviours = new List<GameProperties.KaijuPersonalities>(){};

    // void Start()
    // {
    //     SetUp();
    // }

    void OnEnable()
    {
        MatchFlowManager.CPUMoveRequested += SelectMove;
        MatchFlowManager.CPUSubSystemTargetsRequested += SelectSubSystemTargets;
        MatchFlowManager.CPUDirectionRequested += SetRelativeDirection;
    }

    void OnDisable()
    {
        MatchFlowManager.CPUMoveRequested -= SelectMove;
        MatchFlowManager.CPUSubSystemTargetsRequested -= SelectSubSystemTargets;
        MatchFlowManager.CPUDirectionRequested -= SetRelativeDirection;
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
        CPUMove = KaijuMovePool[0];
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
            bool canDoSomething = CheckForNonZero(MoveTypeProbabilityRatioModifiers);
            if (canDoSomething)
            {
                List<SOFightMoves> filteredListE = FilterByActionType(filteredListD, MoveTypeProbabilityRatioModifiers);
                if(filteredListE.Count > 0)
                {
                    CPUMove = PickAMove(filteredListE);  
                    // Debug.Log($"Move selected: {CPUMove.Name}");  
                    return CPUMove;
                }
                // else
                    // Debug.Log($"FilterByActionType gave no moves");
            }
            // else
                // Debug.Log($"Kaiju can only do Nothing");
        }
        // else
            // Debug.Log($"filteredListD only had one move: {filteredListD[0]}");

        return KaijuMovePool[0]; // Assume kaiju can only do Nothing //  Assumes index 0 contains SOFightMoves called "Nothing".
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
        GameProperties.ActionType[] actionTypes = (GameProperties.ActionType[])System.Enum.GetValues(typeof(GameProperties.ActionType));
        int[] probabilityRatios = new int [actionTypes.Length]; 

        // Set ratios to 0
        for(int i = 0; i < probabilityRatios.Length; i++)
            probabilityRatios[i] = 0;
        
        // Give non-zero positive value to the corresponding action type ratio IF a single move of that type is found.
        foreach(GameProperties.ActionType actionType in actionTypes)
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

        // for(int i = 0; i < probabilityRatios.Length; i++)
        //     Debug.Log($"{(GameProperties.ActionType)i} modified ratio value: {probabilityRatios[i]}");
        
        return probabilityRatios;
    }

    bool CheckForNonZero(int[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            if(array[i] > 0)
                return true;
        }
        //SelectMove() - if all of array is 0, then the only move that the kaiju can do is Nothing.
        //
        return false;
    }

    List<SOFightMoves> FilterByActionType(List<SOFightMoves> movePool, int[] probabilityRatios)
    {
        List<SOFightMoves> filteredList = new List<SOFightMoves>(movePool);
        int[] probabilityUpperBounds = new int[System.Enum.GetValues(typeof(GameProperties.ActionType)).Length];
        int randInt;
        GameProperties.ActionType targetType = GameProperties.ActionType.None;

        for(int i = 0; i < probabilityRatios.Length; i++)
        {
            if(i != 0)
                probabilityUpperBounds[i] = probabilityUpperBounds[i - 1] + probabilityRatios[i];
            else
                probabilityUpperBounds[i] = probabilityRatios[i];
        }

        randInt = Random.Range(0, probabilityUpperBounds[probabilityUpperBounds.Length - 1]);
        for(int i = 0; i < probabilityRatios.Length; i++)
        {
            if(i != 0)
            {
                if(probabilityRatios[i] != 0 && randInt > probabilityUpperBounds[i - 1] && randInt <= probabilityUpperBounds[i])
                    targetType = (GameProperties.ActionType)i;          
            }
            else
            {
                if(probabilityRatios[i] != 0 && randInt > 0 && randInt <= probabilityUpperBounds[i])
                    targetType = (GameProperties.ActionType)i;
            }
        }

        foreach(SOFightMoves move in movePool)
        {
            if(move.ActionType != targetType)
                filteredList.Remove(move);
        }

        // for(int i = 0; i < filteredList.Count; i++)
        //     Debug.Log($"{targetType} move: {filteredList[i]}");

        return filteredList;
    }

    SOFightMoves PickAMove(List<SOFightMoves> movePool)
    {
        List<SOFightMoves> filteredList = new List<SOFightMoves>(movePool);
        int amountOfMoves = movePool.Count;
        if(amountOfMoves > 1)
        {
            int randInt = Random.Range(0,amountOfMoves);
            return movePool[randInt];
        }
        else
            return movePool[0];
    }

    int[] SelectSubSystemTargets()
    {
        int[] subSystemTargets = {0, 0, 0, 0, 0};
        
        subSystemTargets = SetDefaultAndExtraTargets();

        return subSystemTargets;
    }

    int[] SetDefaultAndExtraTargets()
    {
        int[] subSystemTargets = {0, 0, 0, 0, 0};
        GameProperties.ActionType actionType = CPUMove.ActionType;

        switch(actionType)
        {
            case GameProperties.ActionType.Attack:
            {
                subSystemTargets = SetTargetsOpponent();
                break;
            }
            case GameProperties.ActionType.Defend:
            {
                subSystemTargets = SetTargetsSelfDefense();
                break;
            }
            case GameProperties.ActionType.Repair:
            {
                subSystemTargets = SetTargetsSelfRepair();
                break;
            }
            default:    //ActionType is Move, Special, or None. Note that type Move is handled externally in MatchFlowManager.GetCPUMoveAndTargets()
                break;
        }

        return subSystemTargets;
    }

    int[] SetTargetsOpponent()
    {
        int[] subSystemTargets = {0, 0, 0, 0, 0};
        subSystemTargets = SetDefaultTargets(subSystemTargets);
        subSystemTargets = SelectExtraTargets(subSystemTargets, true);

        return subSystemTargets;
    }

    int[] SetTargetsSelfDefense()
    {
        int[] subSystemTargets = {0, 0, 0, 0, 0};
        subSystemTargets = SetDefaultTargets(subSystemTargets);
        subSystemTargets = SelectExtraTargets(subSystemTargets, false);

        return subSystemTargets;
    }

    int[] SetTargetsSelfRepair()
    {
        int[] subSystemTargets = {0, 0, 0, 0, 0};
        subSystemTargets = SetDefaultTargets(subSystemTargets);
        subSystemTargets = SelectExtraTargets(subSystemTargets, false);

        //subSystemTargets = SetDefaultTargets(); Set to 0 any subsystems that have 0 health

        return subSystemTargets;
    }

    int[] SetDefaultTargets(int[] subSystemTargets)
    {
        for(int i = 0; i < CPUMove.DefaultSubSystemTargets.Length; i++)
            subSystemTargets[i] = CPUMove.DefaultSubSystemTargets[i];
        return subSystemTargets;
    }

    int[] SelectExtraTargets(int[] subSystemTargets, bool isAttack)
    {
        if(CPUMove.HasExtraTargets[0] != 0)
        {
            if(isAttack)
                return SelectRandomTargets(subSystemTargets);
            else
            {
                return TriageTargets(subSystemTargets);
            }
        }
        return subSystemTargets;
    }

    int[] SelectRandomTargets(int[] subSystemTargets)
    {
        for(int i = 0; i < CPUMove.HasExtraTargets[1]; i++)
        {
            List<int> nonDefaultSubSystemIndexes = new List<int>(){0, 1, 2, 3, 4};

            foreach(int bodyPart in subSystemTargets)
                nonDefaultSubSystemIndexes.Remove(bodyPart);

            for(int j = 0; j < CPUMove.HasExtraTargets[1]; j++)
            {
                int rand = Random.Range(0,nonDefaultSubSystemIndexes.Count-1);
                subSystemTargets[nonDefaultSubSystemIndexes[rand]] = 1;
                nonDefaultSubSystemIndexes.Remove(rand);
            }
        }
        return subSystemTargets;
    }

    int[] TriageTargets(int[] subSystemTargets)
    {
        float[] healthPercentages = new float[subSystemTargets.Length];
        for(int i = 0; i < subSystemTargets.Length; i++)
        {    
            healthPercentages[i] = MFM.Fighters[1].GetSystemHealthPercentage(i+1);
            // Debug.Log($"System {i} health: {healthPercentages[i].ToString("0.00")}");
        } 
        
        if(CheckForNonZeroFloat(healthPercentages))
        {
            
            bool[] doNotSelect = new bool[subSystemTargets.Length];
            for(int i = 0; i < subSystemTargets.Length; i++)
                doNotSelect[i] = false;
            
            for(int i = 0; i < CPUMove.HasExtraTargets[1]; i++)
            {
                float smallestNonZeroValue = 101f;
                int smallestNonZeroValueIndex = -1;
                for(int j = 0; j < healthPercentages.Length; j++)
                {
                    if(healthPercentages[j] > 0 && !doNotSelect[j] && smallestNonZeroValue > healthPercentages[j])
                    {    
                        smallestNonZeroValue = healthPercentages[j];
                        smallestNonZeroValueIndex = j;
                    }
                }
                if(smallestNonZeroValueIndex > -1)   // This conditional failing should not be possible
                {   
                    subSystemTargets[smallestNonZeroValueIndex] = 1;
                    doNotSelect[smallestNonZeroValueIndex] = true;
                }
            }
        }
        return subSystemTargets;
    }

    bool CheckForNonZeroFloat(float[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            if(array[i] > 0)
                return true;
        }
        //SelectMove() - if all of array is 0, then the only move that the kaiju can do is Nothing.
        //
        return false;
    }

    int[] SetRelativeDirection()
    {
        int[] RelativeDirectionData = {1, 1}; // Index 0 is PlayerID; index 1 is default direction (relative forward)
        RelativeDirectionData[1] = GameProperties.PersonalitiyDirection[CurrentBehaviour];
        return RelativeDirectionData;
    }
}
