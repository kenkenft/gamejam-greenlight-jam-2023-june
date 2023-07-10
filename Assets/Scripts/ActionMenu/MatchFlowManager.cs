using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFlowManager : MonoBehaviour
{
    public CharacterDisplay[] fighters; // Assumes index 0 is player; index 1 is opponent
    private int _priorityOutcome = 0;    
    private int[] _positiveDirection = {1, -1},  // Player positve direction is right-ward and index 0; CPU positve direction is left-ward and index 1
                    _forwardsOrBackwards = {1, 1}, // Store relative direction for player and CPU. Index 0 is player; index 1 is CPU. Set to 1 for relative forward direction, -1 for relative backwards
                    _playerTargetedBodyParts = {0, 0, 0, 0, 0};

    private bool _isSomeoneDead = false, _hasTimerExpired = false;
    private bool[] _areAllSusbSytemsDestroyed = {false, false},
                    _isMainHealthZero = {false, false};

    public SOFightMoves PlayerMove, CPUMove;
    public SOFightMoves[] CPUMoveList;
    private SOFightMoves[] _selectedFightMoves = new SOFightMoves[2];

    // [HideInInspector] public delegate int StringForInt(string targetCharacterProperty, string dictKey = "None");
    // [HideInInspector] public static StringForInt PlayerStatusRequested; 
    // [HideInInspector] public static StringForInt CPUStatusRequested; 

    [HideInInspector] 
    public delegate void SendIntArray(int[] data);
    public static SendIntArray ButtonStatusUpdated;  
    public static SendIntArray MovementCommited;
    public delegate bool IntForBool(int data);
    public static IntForBool TileOccupationRequested; 
    public static IntForBool CheckTileWithinRangeRequested;
    public delegate bool BoolRequested();
    public static BoolRequested TurnHasEnded;
    
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
        // Debug.Log("MatchFlowManager.SetUp called");
        _areAllSusbSytemsDestroyed = new bool[] {false, false};
        _isMainHealthZero = new bool[] {false, false};
        RemoveTempEffects();
        GenerateEnergy();
        StartNextTurn();
    }

    void SetPlayerTargetedParts(int[] targetedParts)
    {
        _playerTargetedBodyParts = targetedParts;
        // Debug.Log("SetPlayerTargetedParts called. {" + _playerTargetedBodyParts[0] + ", " + _playerTargetedBodyParts[1] + ", " + _playerTargetedBodyParts[2] + ", " + _playerTargetedBodyParts[3] + ", " + _playerTargetedBodyParts[4] +"}");
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
            // CheckCharacterDead();
            
            
            GenerateEnergy();
            // PassiveHealing(); // ToDo
            StartNextTurn(); // ToDo
        }
        else
        {
            // CheckEndConditions();
            if(_hasTimerExpired)
                Debug.Log("Game Over. Time has expired");
            if(_isSomeoneDead)
                Debug.Log("Game Over. Player has died");
            // ToDo determine detailed cause of death i.e. All subsystems destroyed and/or main health is zero
        }
    }

    SOFightMoves ComputerMove()
    {
        return CPUMoveList[Random.Range(0, CPUMoveList.Length - 1)];
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
        if(fighters[playerID].CheckWhichHeadBuff((int)GameProperties.BuffTypes.Move) == (int)GameProperties.BuffTypes.Move)
            return movePriority + 1;
        else
            return movePriority;
    }

    void CheckHyperArmourCapabilities()
    {
        fighters[0].SetTempEffectActive(0, PlayerMove.TempEffects[0]);
        fighters[1].SetTempEffectActive(0, CPUMove.TempEffects[0]);
    }

    void ResolveMoves()
    {
        // bool isSomeoneDead = false;
        // Remove move's energy cost from character's energy pool regardless of outcome
        fighters[0].ConsumeEnergy(PlayerMove.Requirements[0]);
        // fighters[0].EnergyData[0] -= PlayerMove.Requirements[0]; 
        fighters[1].ConsumeEnergy(CPUMove.Requirements[0]);

        switch(_priorityOutcome)
        {
            case 0:
            {
                // Debug.Log("Player moves first");
                _selectedFightMoves[0] = PlayerMove;
                _selectedFightMoves[1] = CPUMove;
                ApplyMove(0, fighters[0].HealthBarNum);
                _isSomeoneDead = CheckCharacterDead(1);
                if(!_isSomeoneDead)
                {    
                    ApplyMove(1, fighters[1].HealthBarNum);
                    _isSomeoneDead = CheckCharacterDead(0);
                }
                break;
            }
            case 1:
            {
                // Debug.Log("Opponent moves first");
                _selectedFightMoves[0] = CPUMove;
                _selectedFightMoves[1] = PlayerMove;
                ApplyMove(0, fighters[1].HealthBarNum);
                _isSomeoneDead = CheckCharacterDead(0);
                if(!_isSomeoneDead)
                {    
                    ApplyMove(1, fighters[0].HealthBarNum);
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
                // Debug.Log("Player " + playerID + " move type: BUFF");
                ApplyBuff(playerID, orderIndex);    // Assumes MainEffectValue[0] is the buffType id
                break;
            }
            case 1: // Defensive move
            {
                // Debug.Log("Player " + playerID + " move type: DEFENSIVE");
                ApplyDefense(playerID);
                break;
            }
            case 2: // Movement
            {
                // Debug.Log("Player " + playerID + " move type: MOVEMENT");
                MoveCharacter(playerID, orderIndex);
                break;
            }
            case 3: // Modify Health
            {
                // Debug.Log("Player " + playerID + " move type: MODIFY HEALTH");
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
        fighters[playerID].SetActiveHeadBuff(_selectedFightMoves[orderIndex].MainEffectValue[0]);   // Assumes MainEffectValue is the buffType id
        fighters[playerID].EnergyData[1] = _selectedFightMoves[orderIndex].SecondaryEffects[1]; // Update buff's energy cost
        Debug.Log("Player " + playerID + " active buff: " + (GameProperties.BuffTypes)fighters[playerID].GetActiveHeadBuff() + ". Energy cost: " +fighters[playerID].EnergyData[1]);
    }

    void ApplyDefense(int playerID)
    {
        fighters[playerID].SetTempEffectActive(1, 1);
    }

    void MoveCharacter(int playerID, int orderIndex)
    {
        bool canMoveUnimpeded = true;
        // Retrieve character current tile
        int currentTileID = fighters[playerID].CurrentTileID, 
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
                    // Debug.Log("Player cannot move. Tile" + targetTile + ": \nIsOccupied: " + TileOccupationRequested.Invoke(targetTile));
                    return false;
                }
            }
            else
            {    
                // Debug.Log("Player cannot move. Tile" + targetTile + " IsOutOfrange: " + CheckTileWithinRangeRequested?.Invoke(targetTile));
                return false;
            }
        }
        // Debug.Log("Player can move");
        return true;
    }
    void ModifyHealth(int playerID, int orderIndex)
    {
            int[] healthChanges = {0, 0, 0, 0, 0, 0}; 
            int fighterIndex = playerID;
            // Check which body parts are default targets and extra targets       
            List<int> targetedBodyPartIndexes = GetTargetedSubSystems(orderIndex);                        

            //Only apply defense calculations if attack move
            if(_selectedFightMoves[orderIndex].ActionType == GameProperties.ActionType.Attack)
            {
                fighterIndex = 1 - playerID;
                healthChanges = CalculateRawDamage(_selectedFightMoves[orderIndex].MainEffectValue[0], fighterIndex, targetedBodyPartIndexes);
                healthChanges = ApplyDefenseReductions(orderIndex, playerID, healthChanges);

                //healthChanges' values will subtract from health when calling UpdateCharacterHealth
                for(int i = 0; i < healthChanges.Length; i++)
                    healthChanges[i] *= -1;
            }
            else
                healthChanges = CalculateRawDamage(_selectedFightMoves[orderIndex].MainEffectValue[0], fighterIndex, targetedBodyPartIndexes);
            // ToDo ApplySecondary effects
            fighters[fighterIndex].UpdateCharacterHealth(healthChanges);
        // }
    } // End of ModifyHealth

    int[] CalculateRawDamage(int attackPercentage, int fighterIndex, List<int> targetedSubsystems)
    {
        int[] rawDamage = {0, 0, 0, 0, 0, 0}; 

        for(int i = 0; i < targetedSubsystems.Count; i++)
        {    
            int tempInt = targetedSubsystems[i] + 1;
            rawDamage[tempInt] =  (attackPercentage * fighters[fighterIndex].HealthSystemData[(tempInt * 2)]) / 100;
            rawDamage[0] += rawDamage[tempInt];
        };
        return rawDamage;
    }   // End of CalculateRawDamage

    int[] ApplyDefenseReductions(int orderIndex, int playerID, int[] potentialDamage)
    {
        // Apply bonus defense reduction from HeadBufff: Defense 
        if(fighters[1 - playerID].GetActiveHeadBuff() == (int)GameProperties.BuffTypes.Defense)
        {
            for(int i = 0; i < potentialDamage.Length; i++)
                potentialDamage[i] -= ((60 * potentialDamage[i]) / 100);
        }
            
        // Check if opponent is blocking
        if(fighters[1-playerID].GetTempEffectActive(1) == 1)
        {
            
            int blockThreshold = (_selectedFightMoves[1 - orderIndex].MainEffectValue[1] * fighters[1 - playerID].HealthSystemData[0]) / 100;
            if(potentialDamage[0] <= blockThreshold)
            {
                for(int i = 0; i < potentialDamage.Length; i++)
                    potentialDamage[i] -= (_selectedFightMoves[1 - orderIndex].MainEffectValue[0] * potentialDamage[i]) / 100;
            }
            else
            {
                fighters[1 - playerID].SetTempEffectActive(3, 1);
                for(int i = 0; i < potentialDamage.Length; i++)
                    potentialDamage[i] += (_selectedFightMoves[1 - orderIndex].SecondaryEffects[2] * potentialDamage[i]) / 100;
            } 
        }
        return potentialDamage;
    } // End of ApplyDefenseReductions

    
    List<int> GetTargetedSubSystems(int orderIndex)
    {
        List<int> targetedBodyPartIndexes = new List<int>();
        for(int i = 0; i < _selectedFightMoves[orderIndex].DefaultSubSystemTargets.Length; i++)
        {
            if(_selectedFightMoves[orderIndex].DefaultSubSystemTargets[i] == 1 || _playerTargetedBodyParts[i] == 1)
                targetedBodyPartIndexes.Add(i);
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
        return targetedBodyPartIndexes;
    }

    void RemoveTempEffects()
    {
        foreach(CharacterDisplay fighter in fighters)
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

        int[] systemHealthData = fighters[playerID].HealthSystemData;
        
        // Check whether all subsystems are at zero health
        for(int j = 3; j < systemHealthData.Length; j += 2)
        {
            if(systemHealthData[j] != 0)
                break;
            else if(j == systemHealthData.Length - 1 && systemHealthData[j] == 0) //All subsystems destroyed i.e. character is unable to fight
                _areAllSusbSytemsDestroyed[playerID] = true;
        }

        _isMainHealthZero[playerID] = fighters[playerID].HealthSystemData[1] == 0;

        //Check whether overall hp has dropped to 0
        if(_areAllSusbSytemsDestroyed[playerID] || _isMainHealthZero[playerID])
        {    
            Debug.Log("Player " + playerID + " has died.");
            return true;  
        } 
        else
        {
            Debug.Log("Player " + playerID + " is still alive.");
            return false;  
        }

    }

    void StartNextTurn()
    {
        // ToDo scripts for any tutorial or cutscene 
        // e.g. CutSceneTriggered?.Invoke();

        
    }
    void UpdateButtonUI()
    {
        int[] playerEnergyAndSubSystemsData = {0, 0, 0, 0, 0, 0};
        playerEnergyAndSubSystemsData[0] = fighters[0].EnergyData[0];
        // Debug.Log("EnergyStored: "+ playerEnergyAndSubSystemsData[0]);

        for(int i = 1; i < playerEnergyAndSubSystemsData.Length ; i++)
        {
            int index = (i * 2) + 1 ;
            if(fighters[0].HealthSystemData[index] > 0)
                playerEnergyAndSubSystemsData[i] = 1;

            // Debug.Log("playerEnergyAndSubSystemsData[" + i +"]: "+ playerEnergyAndSubSystemsData[i]);
        }

        ButtonStatusUpdated?.Invoke(playerEnergyAndSubSystemsData);
    }

    void GenerateEnergy()
    {
        fighters[0].GenerateEnergy();
        fighters[1].GenerateEnergy();
    }

    bool CheckActivePlayerBuff(int targetBuff)
    {
        bool isBuffActive = fighters[0].GetActiveHeadBuff() == targetBuff;
        return isBuffActive;
    }

    void CheckEndConditions()
    {
        if(_areAllSusbSytemsDestroyed[0] || _isMainHealthZero[0])
        {
            // Player has died. You lose
        }
        else if(_areAllSusbSytemsDestroyed[1] || _isMainHealthZero[1])
        {
            // Kaiju is dead. You win
        }
        else if( _hasTimerExpired & _areAllSusbSytemsDestroyed[1] && _isMainHealthZero[1])
        {
            // Timer expired, but kaiju has retreated. You win
        }
    }

}
