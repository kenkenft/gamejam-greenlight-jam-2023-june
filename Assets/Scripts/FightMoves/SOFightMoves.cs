using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fight Move", menuName = "FightMove")]
public class SOFightMoves : ScriptableObject
{
    public string ID, Name, Description;
    public Sprite Icon; 
    
    public enum  PossibleMainTargets{
                                        self, opponent, tile
                                    }; // 0 - Self; 1 - opponnent; 2 - tile
    public PossibleMainTargets MainTarget;

    [Range(0,5)]
    public int Priority; // 0 - Slowest; - 5 Fastest
    [Range(0,10)]
    public int Range;

    public int[] MainEffectValue = new int[5];

    public enum MoveCategories{
                            buff, defend, movement, modifyHealth 
                        };
    public MoveCategories MoveType;

    public int[] DefaultSubSystemTargets = new int[5], // truthy int array {head, chest, lArm, rArm, legs}
                 HasExtraTargets = new int [2], //truthy int array {hasExtraTargets, extraTargetAmount}
                 Requirements = new int[6], // int array {energyCost, needHead, needChest, needLArm, needRArm, needLegs} // needHead/Chest etc: 0 - not needed; 1 - mandatory; 2 - At least one of these parts i.e. logical OR
                 TempEffects = new int[4], // int truthy array {givesHyperArmour, givesBlocking, inflictsFlinch, inflictsKnockback}
                 SecondaryEffects = new int[5]; // int truthy array {hasSecondaryEffect, effectValue}
}
