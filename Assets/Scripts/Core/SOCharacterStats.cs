using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class SOCharacterStats : ScriptableObject
{
    public int[] HPSubSystems = new int[5]; // HPHead, HPChest, HPLeftArm, HPRightArm, HPLegs;
    public int BaseEnergyGeneration;

    public GameProperties.CharacterType CharacterType;
    public SOFightMoves[] MovePool;

    public GameProperties.KaijuPersonalities[] Behaviours;

}
