using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFlowManager : MonoBehaviour
{
    public CharacterDisplay[] fighters; // Assumes index 0 is player; index 1 is opponent
    private int _priorityOutcome = 0;

    public SOFightMoves PlayerMove, CPUMove;
    public SOFightMoves[] CPUMoveList;
    private SOFightMoves[] _selectedFightMoves = new SOFightMoves[2];

    // [HideInInspector] public delegate int StringForInt(string targetCharacterProperty, string dictKey = "None");
    // [HideInInspector] public static StringForInt PlayerStatusRequested; 
    // [HideInInspector] public static StringForInt CPUStatusRequested; 

    [HideInInspector] public delegate void SendIntArray(int[] data);
    [HideInInspector] public static SendIntArray ButtonStatusUpdated;  
    
    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
        ActionButton.FightMoveSelected += SetPlayerMove;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
        ActionButton.FightMoveSelected -= SetPlayerMove;
    }

    void SetUp()
    {
        Debug.Log("MatchFlowManager.SetUp called");
        RemoveTempEffects();
        StartNextTurn();
    }
    
    void SetPlayerMove(SOFightMoves playerMove)
    {
        PlayerMove = playerMove;        
        StartMoveResolution();
    }

    void StartMoveResolution()
    {
        CPUMove = ComputerMove(); // ToDO   //Currently, it randomly chooses a move from available move list
        _priorityOutcome = ResolvePriority();
        CheckHyperArmourCapabilities();
        ResolveMoves();
        // ApplySecondaryEffects(); // ToDo
        RemoveTempEffects();
        CheckCharacterDead();
        // CheckTimeExpire(); // ToDO
        
        // GenerateEnergy(); // ToDo
        // PassiveHealing(); // ToDo
        StartNextTurn(); // ToDo
        // UpdateButtonUI();
    }

    SOFightMoves ComputerMove()
    {
        return CPUMoveList[Random.Range(0, CPUMoveList.Length - 1)];
    }

    int ResolvePriority()
    {
        int priorityPlayer = PlayerMove.Priority + fighters[0].CheckWhichHeadBuff("Move"), 
            priorityCPU = CPUMove.Priority + fighters[1].CheckWhichHeadBuff("Move");

        if( priorityPlayer > priorityCPU)
            return 0;   // Player moves first
        else if(priorityPlayer < priorityCPU)
            return 1;   // Opponent moves first
        else
            return 2;   // Draw
    } 

    void CheckHyperArmourCapabilities()
    {
        fighters[0].SetTempEffectActive(0, PlayerMove.TempEffects[0]);
        fighters[1].SetTempEffectActive(0, CPUMove.TempEffects[0]);
    }

    void ResolveMoves()
    {
        switch(_priorityOutcome)
        {
            case 0:
            {
                Debug.Log("Player moves first");
                _selectedFightMoves[0] = PlayerMove;
                _selectedFightMoves[1] = CPUMove;
                ApplyMove(0, fighters[0].HealthBarNum);
                ApplyMove(1, fighters[1].HealthBarNum);
                break;
            }
            case 1:
            {
                Debug.Log("Opponent moves first");
                _selectedFightMoves[0] = CPUMove;
                _selectedFightMoves[1] = PlayerMove;
                ApplyMove(0, fighters[1].HealthBarNum);
                ApplyMove(1, fighters[0].HealthBarNum);
                break; 
            }
            case 2:
            {
                Debug.Log("Both moves equal priority. Analysing further");
                break; 
            }
            default:
                break;
        } // End of switch(_priorityOutcome)
        
    } // End of ResolveMoves()

    void ApplyMove(int orderIndex, int playerID)
    {
        // SOFightMoves targetMove;
        // if(playerID == 0)
        //     targetMove = PlayerMove;
        // else
        //     targetMove = CPUMove;

        switch((int)_selectedFightMoves[orderIndex].MoveType)
        {
            case 0: // Buff
            {
                Debug.Log("Player " + playerID + " move type: BUFF");
                ApplyBuff(playerID, _selectedFightMoves[orderIndex].MainEffectValue[0]);    // Assumes MainEffectValue[0] is the buffType id
                break;
            }
            case 1: // Defensive move
            {
                Debug.Log("Player " + playerID + " move type: DEFENSIVE");
                ApplyDefense(playerID);
                break;
            }
            case 2: // Movement
            {
                Debug.Log("Player " + playerID + " move type: MOVEMENT");
                // MoveCharacter();
                break;
            }
            case 3: // Modify Health
            {
                Debug.Log("Player " + playerID + " move type: MODIFY HEALTH");
                ModifyHealth(orderIndex, playerID);
                break;
            }
            default:
            {
                Debug.Log("The move is neither a buff, defense, movement, or health modifier :(");
                break;
            }
        }
    } // End of ApplyMove()

    void ApplyBuff(int playerID, int buffTypeID)
    {
        fighters[playerID].SetActiveHeadBuff(buffTypeID);   // Assumes MainEffectValue is the buffType id
    }

    void ApplyDefense(int playerID)
    {
        fighters[playerID].SetTempEffectActive(1, 1);
    }

    void ModifyHealth(int orderIndex, int playerID)
    {
        // CheckTarget()
        if((int)_selectedFightMoves[orderIndex].MainTarget == 0)
        {
            // Healing move
            Debug.Log("Healing move selected. Player " + playerID);
        }
        else
        {
            // Attacking move
            int[] damage = {0, 0, 0, 0, 0, 0}; 
            
            // Check which body parts are default targets and extra targets       
            List<int> targetedBodyPartIndexes = GetTargetedSubSystems(orderIndex);                        
            damage = CalculateRawDamage(orderIndex, playerID, targetedBodyPartIndexes);
            damage = ApplyDefenseReductions(orderIndex, playerID, damage);

            for(int i = 0; i < damage.Length; i++)
                damage[i] *= -1;

            // ToDo ApplySecondary effects
            fighters[1 - playerID].UpdateCharacterHealth(damage);
        }
    } // End of ModifyHealth

    int[] ApplyDefenseReductions(int orderIndex, int playerID, int[] potentialDamage)
    {
        // Apply bonus defense reduction from HeadBufff: Defense 
        if(fighters[1 - playerID].GetActiveHeadBuff() == "Defense")
        {
            for(int i = 0; i < potentialDamage.Length; i++)
                potentialDamage[i] -= ((60 * potentialDamage[i]) / 100);
        }
            
        // Check if opponent is blocking
        if(fighters[1-playerID].GetTempEffectActive(1) == 1)
        {
            
            int blockThreshold = (_selectedFightMoves[1 - orderIndex].MainEffectValue[1] * fighters[1 - playerID].HealthSystemData[0]) / 100;
            // Debug.Log("Opponent is blocking! Threshold: " + blockThreshold);

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

    int[] CalculateRawDamage(int orderIndex, int playerID, List<int> targetedSubsystems)
    {
        int attackPercentage = _selectedFightMoves[orderIndex].MainEffectValue[0];
        int[] rawDamage = {0, 0, 0, 0, 0, 0}; 
        for(int i = 0; i < targetedSubsystems.Count; i++)
        {    
            int tempInt = targetedSubsystems[i] + 1;
            rawDamage[tempInt] =  (attackPercentage * fighters[1 - playerID].HealthSystemData[(tempInt * 2)]) / 100;
            rawDamage[0] += rawDamage[tempInt];
        };
        return rawDamage;
    }
    List<int> GetTargetedSubSystems(int orderIndex)
    {
        List<int> targetedBodyPartIndexes = new List<int>();
        for(int i = 0; i < _selectedFightMoves[orderIndex].DefaultSubSystemTargets.Length; i++)
        {
            if(_selectedFightMoves[orderIndex].DefaultSubSystemTargets[i] == 1)
                targetedBodyPartIndexes.Add(i);
        }

        //Randomly chooses extra bodyparts to target. Temporarily here until I implement targetting system 
        if(_selectedFightMoves[orderIndex].HasExtraTargets[0] == 1)
        {
            List<int> extraBodyPartIndexes = new List<int>(){0, 1, 2, 3, 4};

            foreach(int bodyPart in targetedBodyPartIndexes)
            {
                extraBodyPartIndexes.Remove(bodyPart);
            } 

            for(int i = 0; i < _selectedFightMoves[orderIndex].HasExtraTargets[1]; i++)
            {
                int rand = Random.Range(0,extraBodyPartIndexes.Count-1);
                targetedBodyPartIndexes.Add(extraBodyPartIndexes[rand]);
                extraBodyPartIndexes.Remove(rand);
            }
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

    void CheckCharacterDead()
    {
        bool[] isFighterDead = {false, false};
        // Check for character deaths in order to trigger endgame
        // If player dies, it is a loss; if only the kaiju dies, then the player wins.
        for(int i = 0; i < fighters.Length; i++)
        {
            int[] systemHealthData = fighters[i].HealthSystemData;
            
            // Check whether all subsystems are at zero health
            for(int j = systemHealthData.Length - 1; j > 1; j -= 2)
            {
                if(systemHealthData[j] != 0)
                    break;
                else if(j == 3 && systemHealthData[j] == 0)
                    isFighterDead[i] = true;
            }

            //Check whether overall hp has dropped to 0
            if(isFighterDead[i] || fighters[i].HealthSystemData[1] == 0)
            {    
                isFighterDead[i] = true;  
                break;  // Exit int i = 0 loop
            } 
        }

        if(!isFighterDead[0] && !isFighterDead [1])
        {
            Debug.Log("Both characters are still alive. Continue match");
        }
        else
        {
            Debug.Log("Someone died. Triggering Endgame");
        }

    }

    void StartNextTurn()
    {
        // ToDo scripts for any tutorial or cutscene 
        // e.g. CutSceneTriggered?.Invoke();

        
        UpdateButtonUI();
    }
    void UpdateButtonUI()
    {
        int[] playerEnergyAndSubSystemsData = {0, 0, 0, 0, 0, 0};
        playerEnergyAndSubSystemsData[0] = fighters[0].EnergyStored;
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

}
