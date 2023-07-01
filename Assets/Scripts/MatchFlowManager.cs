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
        // fighters[0].SetTempEffectActive("HyperArmour", PlayerMove.TempEffects[0]);
        // fighters[1].SetTempEffectActive("HyperArmour", CPUMove.TempEffects[0]);
        Debug.Log("Player HyperArmour Active: " + fighters[0].GetCharacterData("TempEffects", "HyperArmour"));
        Debug.Log("CPU HyperArmour Active: " + fighters[1].GetCharacterData("TempEffects", "HyperArmour"));
    }

    void ResolveMoves()
    {
        switch(_priorityOutcome)
        {
            case 0:
            {
                Debug.Log("Player moves first");
                ApplyMove(0);
                ApplyMove(1);
                break;
            }
            case 1:
            {
                Debug.Log("Opponent moves first");
                ApplyMove(1);
                ApplyMove(0);
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

    void ApplyMove(int playerID)
    {
        SOFightMoves targetMove;
        if(playerID == 0)
            targetMove = PlayerMove;
        else
            targetMove = CPUMove;

        switch((int)targetMove.MoveType)
        {
            case 0: // Buff
            {
                Debug.Log("Player " + playerID + " move type: BUFF");
                break;
            }
            case 1: // Defensive move
            {
                Debug.Log("Player " + playerID + " move type: DEFENSIVE");
                break;
            }
            case 2: // Movement
            {
                Debug.Log("Player " + playerID + " move type: MOVEMENT");
                break;
            }
            case 3: // Modify Health
            {
                Debug.Log("Player " + playerID + " move type: MODIFY HEALTH");
                break;
            }
            default:
            {
                Debug.Log("The move is neither a buff, defense, movement, or health modifier :(");
                break;
            }
        }
    }
}
