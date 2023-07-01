using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFlowManager : MonoBehaviour
{
    public CharacterDisplay[] fighters;
    bool isPlayerFirst = false;
    public SOFightMoves PlayerMove, CPUMove;
    public SOFightMoves[] CPUMoveList;
    
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
        // isPlayerFirst = ResolvePriority(); // ToDO
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


}
