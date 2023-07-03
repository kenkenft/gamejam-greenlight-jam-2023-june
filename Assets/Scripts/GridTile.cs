using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    public bool hasOccupant = false;

    public CharacterDisplay CurrentOccupant;

    public void SetOccupant(bool isStaying, CharacterDisplay occupant = null)
    {
        if(isStaying)
        {
            hasOccupant = true;
            CurrentOccupant = occupant;
            SetOccupantXPosition();
        }
        else
        {
            hasOccupant = false;
            CurrentOccupant = null;
        }
    }

    void SetOccupantXPosition()
    {
        RectTransform OccupantTransform = CurrentOccupant.gameObject.GetComponent<RectTransform>(); 
        RectTransform TileTransform = gameObject.GetComponent<RectTransform>(); 
        
        OccupantTransform.SetParent(TileTransform); // This is needed, otherwise the reposition is weird.
        OccupantTransform.position = TileTransform.position;        
    }
}
