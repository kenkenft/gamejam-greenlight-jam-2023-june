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
        // CheckHyperArmourCapabilities(); // ToDO
        // ResolveOrder(); // ToDO
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
        //Check Player boosts
        
        // _isMoveBuffActive[PlayerStatusRequested("HealthBarNum")] = PlayerStatusRequested.Invoke("HeadBuff", "Move");
        // _isMoveBuffActive[CPUStatusRequested("HealthBarNum")] = CPUStatusRequested.Invoke("HeadBuff", "Move");

        // Debug.Log("Player priority buff active: " + _isMoveBuffActive[PlayerStatusRequested("HealthBarNum")]);
        // Debug.Log("CPU priority buff active: " + _isMoveBuffActive[CPUStatusRequested("HealthBarNum")]);

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


}
