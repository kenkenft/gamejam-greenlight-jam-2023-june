using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryButton : MonoBehaviour
{
    public enum ActionCategories{
                            Attack, Defend, Move, Special 
                        };
    public ActionCategories MoveType;

    [HideInInspector] 
    public delegate void SendInt(int data);
    public static SendInt OnCategoryButtonPressed;

    public void OnClick()
    {
        OnCategoryButtonPressed?.Invoke((int)MoveType);
    }
    
}
