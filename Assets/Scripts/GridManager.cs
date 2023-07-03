using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GridTile[] Tiles; 
    public CharacterDisplay[] Fighters;

    void OnEnable()
    {
        GameManager.RoundHasStarted += SetUp;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
    }

    void SetUp()
    {
        ResetGridTileStates();
        SetInitialPositions();
    }

    void ResetGridTileStates()
    {
        for(int i = 0; i < Tiles.Length; i++)
            Tiles[i].SetOccupant(false);
    }

    void SetInitialPositions()
    {
        int[] middleTiles = FindMiddleTiles();
        for(int i = 0; i < middleTiles.Length; i++)
            MoveCharacterHere(i, middleTiles[i]);
    }

    int[] FindMiddleTiles()
    {
        int tileAmount = Tiles.Length;
        int[] middleTiles = {0, 0};   
        if(tileAmount % 2 == 0)
        {
            middleTiles[1] = (tileAmount / 2);
            middleTiles[0] = middleTiles[1] - 1;
        }
        else
        {
            middleTiles[1] = (int)Mathf.Ceil((tileAmount / 2));
            middleTiles[0] = (int)Mathf.Floor((tileAmount / 2));
        }
        return middleTiles;
    }

    public void MoveCharacterHere(int fighterID, int targetTile)
    {
        // ToDo CharacterDisplay will need a new variable, CurrentTileID, so that when they are moved, the previous tile can be found and set to unoccupied
        // ToDo Get Character's previous tile that they just left, and set to unoccupied
        Tiles[targetTile].SetOccupant(true, Fighters[fighterID]);
    }


}
