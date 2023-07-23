using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnSomeEvent();
public delegate void SendInt(int data);
public delegate void SendIntArray(int[] data);
public delegate int[] IntArrayRequested();
public delegate bool IntForBool(int data);
public delegate bool BoolRequested();
public delegate void SendBool(bool state);

public delegate void FightMoveSent(SOFightMoves fightMove);
public delegate void SOFightMovesRequired(List<SOFightMoves> relevantActions);

public delegate SOFightMoves SOFightMoveRequested();

public class GameProperties
{
    public static Dictionary<string, string> ColourPalleteHex = new Dictionary<string, string>()
        {
            {"MediumGrey", "#848484"},
            {"LightGrey", "#b8b8b8"},
            {"DarkGrey", "#444444"},
            {"Black", "#101010"},
            {"Special", "#3137fd"}
        };

    public static Dictionary<string, Color> ColourPalleteRGBA = new Dictionary<string, Color>()
        {
            {"MediumGrey", new Color((float)132/255, (float)132/255, (float)132/255, 1f)},
            {"LightGrey", new Color((float)184/255, (float)184/255, (float)184/255, 1f)},
            {"DarkGrey", new Color((float)68/255, (float)68/255, (float)68/255, 1f)},
            {"Black", new Color((float)0/255, (float)0/255, (float)0/255, 1f)},
            {"Special", new Color((float)49/255, (float)55/255, (float)253/255, 1f)},
            {"LightBlue", new Color((float)8/255, (float)184/255, (float)184/255, 1f)},
            {"DarkBlue", new Color((float)4/255, (float)68/255, (float)132/255, 1f)}
        };

    public static Dictionary<ActionType, int> ActionTypePriority = new Dictionary<ActionType, int>()
        {
            {ActionType.Attack,0},
            {ActionType.Defend,2},
            {ActionType.Move,1},
            {ActionType.Special,4},
            {ActionType.Repair,3},
            {ActionType.None, 5}
        };
    
    public enum ActionCategories
        {
            Attack, Defend, Move, Special, None 
        };

    public enum ActionType
        {
            Attack, Defend, Move, Special, Repair, None
        };

    public enum TargetType
        {
            Self, Opponent, Grid, None
        };

    public enum SubSystem
        {
            Head, Chest, LArm, RArm, Legs, None
        };

    public enum TargetDirection
        {
            Forward = 1, Backwards = -1, None = 0
        };

    public enum ColorCombo
        {
            Standard, TargetButtonIsDefault, TargetButtonIsOptional, TargetSelected, TargetIsNotSelected, None
        };

    public enum BuffTypes
    {
        None, Repair, Defense, Move, Attack
    };

    public enum CharacterType
    {
        None, Mech, Kaiju
    }

    public static int BattleIndex = 0;

    public static Dictionary<int, int[]> KaijuMoveSetIDs = new Dictionary<int, int[]>()
        {
            {0, new int[] {0, 1, 4, 6}},  // Tutorial battle
            {1, new int[] {0, 1, 3, 4, 6, 
                            9}},  // Weak monster battle
            {2, new int[] {0, 1, 3, 4, 6, 
                            8, 9, 11, 12, 15, 
                            18}},  // Middle monster battle
            {3, new int[] {0, 1, 2, 3, 4, 
                            5, 6, 7, 8, 9, 
                           10, 11, 12, 13, 14, 
                            15, 16, 17, 18, 19,
                           20, 21}}   // Final monster battle
        }; 

    public static Dictionary<int, string> IDNumToActionID = new Dictionary<int, string>()
        {
            {0, "Mech00"}, {1, "Mech01"}, {2, "Mech02"}, {3, "Mech03"}, {4, "Mech04"}, {5, "Mech05"}, {6, "Mech06"}, {7, "Mech07"}, {8, "Mech08"}, {9, "Mech09"},
            {10, "Mech10"}, {11, "Mech11"}, {12, "Mech12"}, {13, "Mech13"}, {14, "Mech14"}, {15, "Mech15"}, {16, "Mech16"}, {17, "Mech17"}, {18, "Mech18"}, {19, "Mech19"},
            {20, "Mech10"}, {21, "Mech11"}, {22, "Mech12"}, {23, "Mech13"}, {24, "Mech24"}, {25, "Mech25"}, {26, "Mech26"}, {27, "Mech27"}, {28, "Mech28"}, {29, "Mech29"},
        };

    public enum KaijuPersonalities
    {
        Neutral, Aggresive, Defensive, Flighty, BuffOrientated, Healing, 
        Cautious, Desperate, ChangeBuff, Timid, Sedentary, StoneWall,

        OnlyAttack, OnlyDefense, OnlyMove, OnlySpecial, OnlyRepair, OnlyNothing
    };

    public static Dictionary<KaijuPersonalities, int[]> PersonalitiyModifier = new Dictionary<KaijuPersonalities, int[]>
    {
        // Assume int[] is size 6 and order is based on ActionType i.e. Attack, Defend, Move, Special, Repair, None
        {KaijuPersonalities.Neutral, new int[] {0, 0, 0, 0, 0, 0}},
        {KaijuPersonalities.Aggresive, new int[] {500, 0, 0, 0, 0, 0}},
        {KaijuPersonalities.Defensive, new int[] {0, 200, 0, 0, 0, 0}},
        {KaijuPersonalities.Flighty, new int[] {0, 0, 200, 0, 0, 0}},
        {KaijuPersonalities.BuffOrientated, new int[] {0, 0, 0, 200, 0, 0}},
        {KaijuPersonalities.Healing, new int[] {0, 0, 0, 0, 200, 0}},

        {KaijuPersonalities.Cautious, new int[] {-50, 50, 50, 0, 50, 0}},
        {KaijuPersonalities.Desperate, new int[] {500, -90, 300, -90, 0, 0}},
        {KaijuPersonalities.ChangeBuff, new int[] {-99, -99, -99, 1000, -99, 0}},
        {KaijuPersonalities.Timid, new int[] {-50, -50, 100, 0, 0, 0}},
        {KaijuPersonalities.Sedentary, new int[] {0, 0, -99, 0, 0, 0}},
        {KaijuPersonalities.StoneWall, new int[] {50, 200, -99, 0, 50, 0}},

        {KaijuPersonalities.OnlyAttack, new int[] {0, -100, -100, -100, -100, 0}},
        {KaijuPersonalities.OnlyDefense, new int[] {-100, 0, -100, -100, -100, 0}},
        {KaijuPersonalities.OnlyMove, new int[] {-100, -100, 0, -100, -100, -0}},
        {KaijuPersonalities.OnlySpecial, new int[] {-100, -100, -100, 0, -100, 0}},
        {KaijuPersonalities.OnlyRepair, new int[] {-100, -100, -100, -100, 0, 0}},
        {KaijuPersonalities.OnlyNothing, new int[] {-100, -100, -100, -100, -100, 0}},


    };

    public static Dictionary<KaijuPersonalities, int> PersonalitiyDirection = new Dictionary<KaijuPersonalities, int>
    {
        {KaijuPersonalities.Neutral, (int)TargetDirection.Forward},
        {KaijuPersonalities.Aggresive, (int)TargetDirection.Forward},
        {KaijuPersonalities.Defensive, (int)TargetDirection.Backwards},
        {KaijuPersonalities.Flighty, (int)TargetDirection.Backwards},
        {KaijuPersonalities.BuffOrientated, (int)TargetDirection.Backwards},
        {KaijuPersonalities.Healing, (int)TargetDirection.Backwards},

        {KaijuPersonalities.Cautious, (int)TargetDirection.Backwards},
        {KaijuPersonalities.Desperate, (int)TargetDirection.Forward},
        {KaijuPersonalities.ChangeBuff, (int)TargetDirection.Backwards},
        {KaijuPersonalities.Timid, (int)TargetDirection.Backwards},
        {KaijuPersonalities.Sedentary, (int)TargetDirection.Backwards},
        {KaijuPersonalities.StoneWall, (int)TargetDirection.Forward},

        {KaijuPersonalities.OnlyAttack, (int)TargetDirection.Forward},
        {KaijuPersonalities.OnlyDefense, (int)TargetDirection.Backwards},
        {KaijuPersonalities.OnlyMove, (int)TargetDirection.Forward},
        {KaijuPersonalities.OnlySpecial, (int)TargetDirection.Forward},
        {KaijuPersonalities.OnlyRepair, (int)TargetDirection.Backwards},
        {KaijuPersonalities.OnlyNothing, (int)TargetDirection.Forward},

    };
} // End of Class