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
        // Vector2 newAnchorPosition = CurrentOccupant.gameObject.GetComponent<RectTransform>().anchoredPosition;
        // newAnchorPosition[0] = gameObject.GetComponent<RectTransform>().anchoredPosition[0];

        // CurrentOccupant.gameObject.GetComponent<RectTransform>().anchoredPosition = newAnchorPosition;

        RectTransform OccupantTransform = CurrentOccupant.gameObject.GetComponent<RectTransform>(); 
        Vector3 newPosition = OccupantTransform.position;
        OccupantTransform.SetParent(this.gameObject.GetComponent<RectTransform>());
        Debug.Log("Current position X:" + newPosition[0] + " Y: " + newPosition[1] + " Z: " + newPosition[2]);

        // newPosition[0] = this.gameObject.GetComponent<RectTransform>().position.x;
        // newPosition[0] = 1f;
        // OccupantTransform.position = newPosition;
        OccupantTransform.position = gameObject.GetComponent<RectTransform>().position;

        Debug.Log("New position X:" + OccupantTransform.position.x + " Y: " + OccupantTransform.position.y + " Z: " + OccupantTransform.position.z);
        
    }
}
