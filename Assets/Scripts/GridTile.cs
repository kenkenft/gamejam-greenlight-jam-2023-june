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
            SetOccupantPosition();
        }
        else
        {
            hasOccupant = false;
            CurrentOccupant = null;
        }
    }

    void SetOccupantPosition()
    {
        // Issue: The y position of the characters is inconsistent when this is called outside the intial setup. I don't know why that happens 
        RectTransform OccupantTransform = CurrentOccupant.gameObject.GetComponent<RectTransform>(), 
                        TileTransform = gameObject.GetComponent<RectTransform>();
        Vector3 newOccupantPosition = TileTransform.position; 
        newOccupantPosition[1] = 0f;
        
        OccupantTransform.SetParent(TileTransform); // This is needed, otherwise the reposition is weird.
        OccupantTransform.localPosition = newOccupantPosition; // method 2. haven't tested yet
        // OccupantTransform.position = newOccupantPosition; //method 1. Doesn't seem to work after initial setup.


    }
}
