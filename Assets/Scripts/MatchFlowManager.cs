using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFlowManager : MonoBehaviour
{
    public CharacterDisplay[] fighters; // Assumes index 0 is player; index 1 is opponent
    private int _priorityOutcome = 0;
    private int[] _isMoveBuffActive = {0,0};    //truthy values. 0 is inactive/false; 1 is active/enabled
    enum PriorityOutcome{
                            player, opponent, draw
                        };
    public SOFightMoves PlayerMove, CPUMove;
    public SOFightMoves[] CPUMoveList;
    private SOFightMoves[] _selectedFightMoves = new SOFightMoves[2];

    [HideInInspector] public delegate int StringForInt(string targetCharacterProperty, string dictKey = "None");
    [HideInInspector] public static StringForInt PlayerStatusRequested; 
    [HideInInspector] public static StringForInt CPUStatusRequested; 
    
    void OnEnable()
    {
        ActionButton.FightMoveSelected += SetPlayerMove;
    }

    void OnDisable()
    {
        ActionButton.FightMoveSelected -= SetPlayerMove;
    }
    
    void SetPlayerMove(SOFightMoves playerMove)
    {
        PlayerMove = playerMove; // ToDO

        // Debug.Log("Player Move Clicked: " + PlayerMove.Name);
        
        StartMoveResolution();
    }

    void StartMoveResolution()
    {
        CPUMove = ComputerMove(); // ToDO
        // Debug.Log("CPU Move: " + CPUMove.Name);
        _priorityOutcome = ResolvePriority(); // ToDO
        // Debug.Log("Priority outcome: " + _priorityOutcome);
        CheckHyperArmourCapabilities(); // ToDO
        ResolveMoves(); // ToDO
        // CheckCharacterDead(); // ToDO
        // CheckTimeExpire(); // ToDO
        // RemoveTempEffects(); // ToDO
        // GenerateEnergy(); // ToDo
        // PassiveHealing(); // ToDo
        // StartNextTurn(); // ToDo
    }

    SOFightMoves ComputerMove()
    {
        return CPUMoveList[Random.Range(0, CPUMoveList.Length - 1)];
    }

    int ResolvePriority()
    {
        // _isMoveBuffActive[PlayerStatusRequested("HealthBarNum")] = PlayerStatusRequested.Invoke("HeadBuff", "Move");
        // _isMoveBuffActive[CPUStatusRequested("HealthBarNum")] = CPUStatusRequested.Invoke("HeadBuff", "Move");

        // Debug.Log("Player priority buff active: " + fighters[0].GetCharacterData("HeadBuffs", "Move"));
        // Debug.Log("CPU priority buff active: " + fighters[1].GetCharacterData("HeadBuffs", "Move"));

        int priorityPlayer = PlayerMove.Priority + fighters[0].GetCharacterData("HeadBuffs", "Move"), 
            priorityCPU = CPUMove.Priority + fighters[1].GetCharacterData("HeadBuffs", "Move");

        if( priorityPlayer > priorityCPU)
            return 0;   // Player moves first
        else if(priorityPlayer < priorityCPU)
            return 1;   // Opponent moves first
        else
            return 2;   // Draw
    } 

    void CheckHyperArmourCapabilities()
    {
        fighters[0].SetTempEffectActive("HyperArmour", PlayerMove.TempEffects[0]);
        fighters[1].SetTempEffectActive("HyperArmour", CPUMove.TempEffects[0]);
        // Debug.Log("Player HyperArmour Active: " + fighters[0].GetCharacterData("TempEffects", "HyperArmour"));
        // Debug.Log("CPU HyperArmour Active: " + fighters[1].GetCharacterData("TempEffects", "HyperArmour"));
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
                ApplyBuff(playerID, _selectedFightMoves[orderIndex].MainEffectValue);    // Assumes MainEffectValue is the buffType id
                break;
            }
            case 1: // Defensive move
            {
                Debug.Log("Player " + playerID + " move type: DEFENSIVE");
                // ApplyDefense();
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
            Debug.Log("Attacking move selected. Player " + playerID + " is attacking other Player");
        }
    }
}
