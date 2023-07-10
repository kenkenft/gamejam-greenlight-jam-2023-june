using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                                                                                                {"Black", new Color((float)16/255, (float)16/255, (float)16/255, 1f)},
                                                                                                {"Special", new Color((float)49/255, (float)55/255, (float)253/255, 1f)}
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
    
    public enum ActionCategories{
                                    Attack, Defend, Move, Special, None 
                                  };

    public enum ActionType{
                                Attack, Defend, Move, Special, Repair, None
                            };

    public enum TargetType{
                                Self, Opponent, Grid, None
                            };

    public enum SubSystem{
                            Head, Chest, LArm, RArm, Legs, None
                            };

    public enum TargetDirection{
                                Forward = 1, Backwards = -1, None = 0
                            };

    public enum ColorCombo{
                        Standard, TargetButtonIsDefault, TargetButtonIsOptional, TargetSelected, TargetIsNotSelected, None
                        };

    public enum BuffTypes{
                        None, Repair, Defense, Move, Attack
                        };
}