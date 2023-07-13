using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFlowManager : MonoBehaviour
{
    public CharacterDisplay[] Fighters; // Assumes index 0 is player; index 1 is opponent
    public CPUMoveSelect CPUMoveSelector;
    private int _priorityOutcome = 0;    
    private int[] _positiveDirection = {1, -1},  // Player positve direction is right-ward and index 0; CPU positve direction is left-ward and index 1
                    _forwardsOrBackwards = {1, 1}, // Store relative direction for player and CPU. Index 0 is player; index 1 is CPU. Set to 1 for relative forward direction, -1 for relative backwards
                    _playerTargetedBodyParts = {0, 0, 0, 0, 0};
    private int[,] _targetedBodyParts = new int[,] {
                                                        {0, 0, 0, 0, 0},    // Targeted by the player
                                                        {0, 0, 0, 0, 0}     // Targeted by opponent
                                                    };

    private bool _isSomeoneDead = false, _hasTimerExpired = false;
    private bool[] _areAllSusbSytemsDestroyed = {false, false},
                    _isMainHealthZero = {false, false};

    public SOFightMoves PlayerMove, CPUMove;
    public SOFightMoves[] CPUMoveList;
    private SOFightMoves[] _selectedFightMoves = new SOFightMoves[2];

    [HideInInspector] 
    public static SendIntArray ButtonStatusUpdated, MovementCommited;
    public static IntForBool TileOccupationRequested, CheckTileWithinRangeRequested;
    public static BoolRequested TurnHasEnded;
    public static SendInt EndConditionsMet, WhichEndingDetermined;
    public static SOFightMoveRequested CPUMoveRequested;
    
    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
        ActionButton.CurrentBuffRequested += CheckActivePlayerBuff;
        ConfirmationButton.AllTargetsConfirmed += SetPlayerTargetedParts;
        ConfirmationButton.RelativeDirectionConfirmed += SetRelativeDirection;
        ConfirmationButton.FightMoveConfirmed += SetPlayerMove;
        CategoryButton.OnCategorySelected += UpdateButtonUI;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
        ActionButton.CurrentBuffRequested -= CheckActivePlayerBuff;
        ConfirmationButton.AllTargetsConfirmed -= SetPlayerTargetedParts;
        ConfirmationButton.RelativeDirectionConfirmed -= SetRelativeDirection;
        ConfirmationButton.FightMoveConfirmed -= SetPlayerMove;
        CategoryButton.OnCategorySelected -= UpdateButtonUI;
    }

    void SetUp()
    {
        _areAllSusbSytemsDestroyed = new bool[] {false, false};
        _isMainHealthZero = new bool[] {false, false};
        Fighters[0].SetUp();
        CPUMoveSelector.SetUp();
        RemoveTempEffects();
        GenerateEnergy();
        StartNextTurn();
    }

    void SetPlayerTargetedParts(int[] targetedParts)
    {
        // _playerTargetedBodyParts = targetedParts;
        SetTargetedParts(0, targetedParts);
    }

    void SetTargetedParts(int playerID, int[] targetedParts)
    {
        for(int i = 0; i < targetedParts.Length; i++)
            _targetedBodyParts[playerID, i] = targetedParts[i];
    }

    int[] GetTargetedParts(int playerID)
    {
        int[] targetedParts = {0, 0, 0, 0, 0};
        for(int i = 0; i < _playerTargetedBodyParts.GetLength(playerID); i++)
            targetedParts[i] = _targetedBodyParts[playerID, i];
        
        return targetedParts;
    }

    void SetRelativeDirection(int[] relativeDirectionData)
    {
        _forwardsOrBackwards[relativeDirectionData[0]] = relativeDirectionData[1]; 
    }
    
    void SetPlayerMove(SOFightMoves playerMove)
    {
        PlayerMove = playerMove;        
        StartMoveResolution();
        // Debug.Log("SetPlayerMove called. Selected move: " + PlayerMove.name);
        // Debug.Log("_forwardsOrBackwards[0]: " + _forwardsOrBackwards[0]);
    }

    void StartMoveResolution()
    {
        CPUMove = ComputerMove(); // ToDO   //Currently, it randomly chooses a move from available move list
        _priorityOutcome = ResolvePriority();
        CheckHyperArmourCapabilities();
        ResolveMoves();

        _hasTimerExpired = TurnHasEnded.Invoke();

        if(!_isSomeoneDead && !_hasTimerExpired)
        {
            // ApplySecondaryEffects(); // ToDo
            RemoveTempEffects();
            GenerateEnergy();
            PassiveHealing(); // ToDo
            StartNextTurn(); // ToDo
        }
        else
        {
            EndConditionsMet?.Invoke(-2);   // Disable Category buttons
            // SetUpEndScreen();
            if(_hasTimerExpired)
                Debug.Log("Game Over. Time has expired");
            if(_isSomeoneDead)
                Debug.Log("Game Over. Player has died");
            SetUpEndScreen();
            // ToDo determine detailed cause of death i.e. All subsystems destroyed and/or main health is zero
        }
    }

    SOFightMoves ComputerMove()
    {
        // return CPUMoveList[Random.Range(0, CPUMoveList.Length - 1)];
        return CPUMoveRequested.Invoke();
        // Invoke delegate that CPUMoveSelect.SelectMove is subscribed to returns 
    }

    int ResolvePriority()
    {
        // int priorityPlayer = PlayerMove.Priority + fighters[0].CheckWhichHeadBuff((int)GameProperties.BuffTypes.Move), 
        //     priorityCPU = CPUMove.Priority + fighters[1].CheckWhichHeadBuff((int)GameProperties.BuffTypes.Move);
        
        int priorityPlayer = ApplyPriorityBonus(PlayerMove.Priority, 0), 
            priorityCPU = ApplyPriorityBonus(CPUMove.Priority, 1),
            outcomeCheck = 0;
        
        // Debug.Log("Move Priority checked first");
        outcomeCheck = CheckPriority(priorityPlayer, priorityCPU);
        if(outcomeCheck != 2)
        {
            return outcomeCheck;
        }
        else
        {
            // Debug.Log("Move Priorities were equal. Checking ActionType rankings");
            priorityPlayer = GameProperties.ActionTypePriority[PlayerMove.ActionType];
            priorityCPU = GameProperties.ActionTypePriority[CPUMove.ActionType];
            outcomeCheck = CheckPriority(priorityPlayer, priorityCPU);

            if(outcomeCheck != 2)
                return outcomeCheck;
            else
            {
                // Debug.Log("Move priority and action type are the same. Defaulted to giving player priority");
                return 0;   // In the event that move priority and action type are the same, player always gets priority.
            }
        }
    } 

    int CheckPriority(int priorityPlayer, int priorityCPU)
    {
        if( priorityPlayer > priorityCPU)
            return 0;   // Player moves first
        else if(priorityPlayer < priorityCPU)
            return 1;   // Opponent moves first
        else
            return 2;   // Both priorities are equal
    }

    int ApplyPriorityBonus(int movePriority, int playerID)
    {
        if(Fighters[playerID].CheckWhichHeadBuff((int)GameProperties.BuffTypes.Move) == (int)GameProperties.BuffTypes.Move)
            return movePriority + 1;
        else
            return movePriority;
    }

    void CheckHyperArmourCapabilities()
    {
        Fighters[0].SetTempEffectActive(0, PlayerMove.TempEffects[0]);
        Fighters[1].SetTempEffectActive(0, CPUMove.TempEffects[0]);
    }

    void ResolveMoves()
    {
        // bool isSomeoneDead = false;
        // Remove move's energy cost from character's energy pool regardless of outcome
        Fighters[0].ConsumeEnergy(PlayerMove.Requirements[0]);
        // fighters[0].EnergyData[0] -= PlayerMove.Requirements[0]; 
        Fighters[1].ConsumeEnergy(CPUMove.Requirements[0]);

        switch(_priorityOutcome)
        {
            case 0:
            {
                // Debug.Log("Player moves first");
                _selectedFightMoves[0] = PlayerMove;
                _selectedFightMoves[1] = CPUMove;
                ApplyMove(0, Fighters[0].HealthBarNum);
                _isSomeoneDead = CheckCharacterDead(1);
                if(!_isSomeoneDead)
                {    
                    ApplyMove(1, Fighters[1].HealthBarNum);
                    _isSomeoneDead = CheckCharacterDead(0);
                }
                break;
            }
            case 1:
            {
                // Debug.Log("Opponent moves first");
                _selectedFightMoves[0] = CPUMove;
                _selectedFightMoves[1] = PlayerMove;
                ApplyMove(0, Fighters[1].HealthBarNum);
                _isSomeoneDead = CheckCharacterDead(0);
                if(!_isSomeoneDead)
                {    
                    ApplyMove(1, Fighters[0].HealthBarNum);
                    _isSomeoneDead = CheckCharacterDead(1);
                }
                break; 
            }
            default:
                break;
        } // End of switch(_priorityOutcome)
        
    } // End of ResolveMoves()

    void ApplyMove(int orderIndex, int playerID)
    {

        switch((int)_selectedFightMoves[orderIndex].MoveType)
        {
            case 0: // Buff
            {
                ApplyBuff(playerID, orderIndex);    // Assumes MainEffectValue[0] is the buffType id
                break;
            }
            case 1: // Defensive move
            {
                ApplyDefense(playerID);
                break;
            }
            case 2: // Movement
            {
                MoveCharacter(playerID, orderIndex);
                break;
            }
            case 3: // Modify Health
            {
                ModifyHealth(playerID, orderIndex);
                break;
            }
            default:
            {
                Debug.Log("The move is neither a buff, defense, movement, or health modifier :(");
                break;
            }
        }
    } // End of ApplyMove()

    void ApplyBuff(int playerID, int orderIndex)
    {
        Fighters[playerID].SetActiveHeadBuff(_selectedFightMoves[orderIndex].MainEffectValue[0]);   // Assumes MainEffectValue is the buffType id
        Fighters[playerID].BuffPrimaryEffect = _selectedFightMoves[orderIndex].MainEffectValue[1];  // Update buff's primary effect value
        Fighters[playerID].EnergyData[1] = _selectedFightMoves[orderIndex].MainEffectValue[2]; // Update buff's energy cost
        
        // Debug.Log("Player " + playerID + " active buff: " + (GameProperties.BuffTypes)fighters[playerID].GetActiveHeadBuff() + ". Energy cost: " +fighters[playerID].EnergyData[1]);
    }

    void ApplyDefense(int playerID)
    {
        Fighters[playerID].SetTempEffectActive(1, 1);
    }

    void MoveCharacter(int playerID, int orderIndex)
    {
        bool canMoveUnimpeded = true;
        // Retrieve character current tile
        int currentTileID = Fighters[playerID].CurrentTileID, 
            movementRange = _selectedFightMoves[orderIndex].MainEffectValue[0],
            overallDirection = _forwardsOrBackwards[playerID] * _positiveDirection[playerID];   // 1 when going rightwards, -1 when going leftwards

        // Check if target tile or tiles on the way to target tile is either occupied or out of indexed tile range
        canMoveUnimpeded = CheckValidTargetTile(currentTileID, movementRange, overallDirection);

        // If occupied or target tile is out of range, then cancel movement. Otherwise, call MovementCommited?.Invoke()
        if(canMoveUnimpeded)
        {
            int[] targetData = {playerID, (currentTileID + (movementRange * overallDirection))};
            // Debug.Log("Player " + targetData[0] + " moving to Tile " + targetData[1]);
            MovementCommited?.Invoke(targetData);
        }
    }

    bool CheckValidTargetTile(int currentTileID, int movementRange, int overallDirection)
    {
        int targetTile = 0;
        for(int i = 1; i <= movementRange; i++)
        {
            targetTile = currentTileID + (i * overallDirection);
            if(CheckTileWithinRangeRequested.Invoke(targetTile))
            {
                if(TileOccupationRequested.Invoke(targetTile))
                {
                    // Debug.Log("Tile" + targetTile + ": \nIsOccupied: " + TileOccupationRequested.Invoke(targetTile));
                    return false;
                }
            }
            else
            {    
                // Debug.Log("Tile" + targetTile + " IsOutOfrange: " + CheckTileWithinRangeRequested?.Invoke(targetTile));
                return false;
            }
        }
        // Debug.Log("No obstacles detected");
        return true;
    }
    void ModifyHealth(int playerID, int orderIndex)
    {
            int[] healthChanges = {0, 0, 0, 0, 0, 0}; 
            int targetID;
            // Check which body parts are default targets and extra targets       
            List<int> filteredTargetedIndexes = GetTargetedSubSystems(playerID, orderIndex);                      

            //Only apply defense calculations if attack move
            if(_selectedFightMoves[orderIndex].ActionType == GameProperties.ActionType.Attack)
            {
                targetID = 1 - playerID;    // Sets ID of opponent as the target 
                if(HitDetection(playerID, orderIndex))
                {
                    
                    healthChanges = CalculateRawDamage(_selectedFightMoves[orderIndex].MainEffectValue[0], targetID, filteredTargetedIndexes);
                    healthChanges = ApplyAttackModifications(orderIndex, playerID, healthChanges);
                    healthChanges = ApplyDefenseReductions(orderIndex, playerID, healthChanges);

                    //healthChanges' values will subtract from health when calling UpdateCharacterHealth
                    for(int i = 0; i < healthChanges.Length; i++)
                        healthChanges[i] *= -1;
                }
            }
            else
            {    
                targetID = playerID;
                List<int> activeTargetedPartsIndexes = RemoveCriticallyDamagedTargets(playerID, filteredTargetedIndexes);
                healthChanges = CalculateRawDamage(_selectedFightMoves[orderIndex].MainEffectValue[0], targetID, activeTargetedPartsIndexes);
            }
            // ToDo ApplySecondary effects
            Fighters[targetID].UpdateCharacterHealth(healthChanges);
        // }
    } // End of ModifyHealth

    bool HitDetection(int playerID, int orderIndex)
    {
        // Retrieve character current tile
        int currentTileID = Fighters[playerID].CurrentTileID, 
            attackRange = _selectedFightMoves[orderIndex].Range;
            // overallDirection = _positiveDirection[playerID];   // 1 when going rightwards, -1 when going leftwards

        // Check if target tile or tiles on the way to target tile is either occupied or out of indexed tile range
        bool isTargetInRange = !(CheckValidTargetTile(currentTileID, attackRange, _positiveDirection[playerID]));   //Inverted output as it returns true when no obstacles present

        Debug.Log($"targetHit: {isTargetInRange}");
        return isTargetInRange;
    }

    int[] CalculateRawDamage(int attackPercentage, int playerID, List<int> targetedSubsystems)
    {
        int[] rawDamage = {0, 0, 0, 0, 0, 0}; 

        for(int i = 0; i < targetedSubsystems.Count; i++)
        {    
            int tempInt = targetedSubsystems[i] + 1;
            rawDamage[tempInt] =  (attackPercentage * Fighters[playerID].HealthSystemData[(tempInt * 2)]) / 100;
            rawDamage[0] += rawDamage[tempInt];
        };
        return rawDamage;
    }   // End of CalculateRawDamage

    int[] ApplyAttackModifications(int orderIndex, int playerID, int[] potentialDamage)
    {
        int AttackBuffModifier = 0;
        bool hasAttackBuffActive = Fighters[playerID].GetActiveHeadBuff() == (int)GameProperties.BuffTypes.Attack;
        
        if(hasAttackBuffActive)
            AttackBuffModifier = Fighters[playerID].BuffPrimaryEffect;

        if(hasAttackBuffActive)
        {    
            for(int i = 0; i < potentialDamage.Length; i++) 
                potentialDamage[i] += (potentialDamage[i] * AttackBuffModifier) / 100;
        }
        return potentialDamage;
    }
    int[] ApplyDefenseReductions(int orderIndex, int playerID, int[] potentialDamage)
    {
        // Apply bonus defense reduction from HeadBufff: Defense 
        if(Fighters[1 - playerID].GetActiveHeadBuff() == (int)GameProperties.BuffTypes.Defense)
        {
            for(int i = 0; i < potentialDamage.Length; i++)
                potentialDamage[i] -= ((Fighters[playerID].BuffPrimaryEffect * potentialDamage[i]) / 100);
        }
            
        // Check if opponent is blocking
        if(Fighters[1-playerID].GetTempEffectActive(1) == 1)
        {
            
            int blockThreshold = (_selectedFightMoves[1 - orderIndex].MainEffectValue[1] * Fighters[1 - playerID].HealthSystemData[0]) / 100;
            if(potentialDamage[0] <= blockThreshold)
            {
                for(int i = 0; i < potentialDamage.Length; i++)
                    potentialDamage[i] -= (_selectedFightMoves[1 - orderIndex].MainEffectValue[0] * potentialDamage[i]) / 100;
            }
            else
            {
                Fighters[1 - playerID].SetTempEffectActive(3, 1);
                for(int i = 0; i < potentialDamage.Length; i++)
                    potentialDamage[i] += (_selectedFightMoves[1 - orderIndex].SecondaryEffects[2] * potentialDamage[i]) / 100;
            } 
        }
        return potentialDamage;
    } // End of ApplyDefenseReductions

    
    List<int> GetTargetedSubSystems(int playerID, int orderIndex)
    {
        List<int> filteredTargetedBodyParts = new List<int>();
        int[] manuallyTargetedParts = GetTargetedParts(playerID);
        for(int i = 0; i < _selectedFightMoves[orderIndex].DefaultSubSystemTargets.Length; i++)
        {
            if(_selectedFightMoves[orderIndex].DefaultSubSystemTargets[i] == 1 || manuallyTargetedParts[i] == 1)  
                filteredTargetedBodyParts.Add(i);
        }
        {
        //Randomly chooses extra bodyparts to target. Temporarily here until I implement targetting system 
        // if(_selectedFightMoves[orderIndex].HasExtraTargets[0] == 1)
        // {
        //     List<int> extraBodyPartIndexes = new List<int>(){0, 1, 2, 3, 4};

        //     foreach(int bodyPart in targetedBodyPartIndexes)
        //     {
        //         extraBodyPartIndexes.Remove(bodyPart);
        //     } 

        //     for(int i = 0; i < _selectedFightMoves[orderIndex].HasExtraTargets[1]; i++)
        //     {
        //         int rand = Random.Range(0,extraBodyPartIndexes.Count-1);
        //         targetedBodyPartIndexes.Add(extraBodyPartIndexes[rand]);
        //         extraBodyPartIndexes.Remove(rand);
        //     }
        // }
        }
        return filteredTargetedBodyParts;
    }

    List<int> RemoveCriticallyDamagedTargets(int playerID, List<int> filteredParts)
    {        
        List<int> temptList = filteredParts;
        int HealthSystemDataIndex = 0;
        bool isDestroyed = false;

        for(int i = 0; i < temptList.Count; i++)
        {
            HealthSystemDataIndex = (temptList[i] * 2) + 3;
            isDestroyed = Fighters[playerID].HealthSystemData[HealthSystemDataIndex] <= 0;
            
            if(isDestroyed)
                filteredParts.Remove(temptList[i]);
        }
        return filteredParts;
    }
    void RemoveTempEffects()
    {
        foreach(CharacterDisplay fighter in Fighters)
        {
            List<string> keyList = new List<string>(fighter.TempEffects.Keys);
            foreach(string key in keyList)
                fighter.TempEffects[key] = 0;
        }
    } // End of RemoveTempEffects

    bool CheckCharacterDead(int playerID)
    {
        // Check for character deaths in order to trigger endgame
        // If player dies, it is a loss; if only the kaiju dies, then the player wins.

        int[] systemHealthData = Fighters[playerID].HealthSystemData;
        
        // Check whether all subsystems are at zero health
        for(int j = 3; j < systemHealthData.Length; j += 2)
        {
            if(systemHealthData[j] != 0)
                break;
            else if(j == systemHealthData.Length - 1 && systemHealthData[j] == 0) //All subsystems destroyed i.e. character is unable to fight
                _areAllSusbSytemsDestroyed[playerID] = true;
        }

        _isMainHealthZero[playerID] = Fighters[playerID].HealthSystemData[1] == 0;

        //Check whether overall hp has dropped to 0
        if(_areAllSusbSytemsDestroyed[playerID] || _isMainHealthZero[playerID])   
            return true;  
        else
            return false;  

    }

    void StartNextTurn()
    {
        // ToDo scripts for any tutorial or cutscene 
        // e.g. CutSceneTriggered?.Invoke();

        
    }
    void UpdateButtonUI()
    {
        int[] playerEnergyAndSubSystemsData = {0, 0, 0, 0, 0, 0};
        playerEnergyAndSubSystemsData[0] = Fighters[0].EnergyData[0];
        // Debug.Log("EnergyStored: "+ playerEnergyAndSubSystemsData[0]);

        for(int i = 1; i < playerEnergyAndSubSystemsData.Length ; i++)
        {
            int index = (i * 2) + 1 ;
            if(Fighters[0].HealthSystemData[index] > 0)
                playerEnergyAndSubSystemsData[i] = 1;

            // Debug.Log("playerEnergyAndSubSystemsData[" + i +"]: "+ playerEnergyAndSubSystemsData[i]);
        }

        ButtonStatusUpdated?.Invoke(playerEnergyAndSubSystemsData);
    }

    void GenerateEnergy()
    {
        Fighters[0].GenerateEnergy();
        Fighters[1].GenerateEnergy();
    }

    bool CheckActivePlayerBuff(int targetBuff)
    {
        bool isBuffActive = Fighters[0].GetActiveHeadBuff() == targetBuff;
        return isBuffActive;
    }

    // void CheckEndConditions()
    void SetUpEndScreen()
    {
        Debug.Log("SetUpEndScreen called");
        if(_areAllSusbSytemsDestroyed[0] || _isMainHealthZero[0])
        {
            // Player has died. You lose
            WhichEndingDetermined?.Invoke(0);
        }
        else if(_areAllSusbSytemsDestroyed[1] || _isMainHealthZero[1])
        {
            // Kaiju is dead. You win
            WhichEndingDetermined?.Invoke(1);
        }
        else
        {
            // Timer expired, but kaiju has retreated. You win
            WhichEndingDetermined?.Invoke(2);
        }
    }

    void PassiveHealing()
    {
        List<int> allSubsystems = new List<int>{0, 1, 2, 3, 4};
        for(int i = 0; i < Fighters.Length; i++)
        {
            if(Fighters[i].GetActiveHeadBuff() == (int)GameProperties.BuffTypes.Repair)
            {
                List<int> filteredTargetedIndexes = RemoveCriticallyDamagedTargets(i, allSubsystems);
                int[] healthChanges = CalculateRawDamage(Fighters[i].BuffPrimaryEffect, i, filteredTargetedIndexes);
                Fighters[i].UpdateCharacterHealth(healthChanges);
            }
        }
        
    }
}
