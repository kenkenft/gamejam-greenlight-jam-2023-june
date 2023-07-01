using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fight Move", menuName = "FightMove")]
public class SOFightMoves : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon; 
    
    [Range(0,3)]
    public int  MainTarget; // 0 - Self; 1 - opponnent; 2 - tile
    [Range(0,5)]
    public int Priority; // 0 - Slowest; - 5 Fastest
    [Range(0,10)]
    public int Range;

    public int MainEffectValue;

    public enum MoveCategories{
                            buff, defend, movement, modifyHealth 
                        };
    public MoveCategories MoveType;

    public int[] DefaultSubSystemTargets = new int[5], // truthy int array {head, chest, lArm, rArm, legs}
                 HasExtraTargets = new int [2], //truthy int array {hasExtraTargets, extraTargetAmount}
                 Requirements = new int[6], // int array {energyCost, needHead, needChest, needLArm, needRArm, needLegs}
                 TempEffects = new int[4], // int truthy array {givesHyperArmour, givesBlocking, inflictsFlinch, inflictsKnockback}
                 SecondaryEffects = new int[2]; // int truthy array {hasSecondaryEffect, effectValue}
}
