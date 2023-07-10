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
        MatchFlowManager.MovementCommited += MoveCharacterHere;
        MatchFlowManager.TileOccupationRequested += IsTileOccupied;
        MatchFlowManager.CheckTileWithinRangeRequested += IsValidTileID;
    }

    void OnDisable()
    {
        GameManager.RoundHasStarted -= SetUp;
        MatchFlowManager.MovementCommited -= MoveCharacterHere;
        MatchFlowManager.TileOccupationRequested -= IsTileOccupied;
        MatchFlowManager.CheckTileWithinRangeRequested -= IsValidTileID;
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
        int[] middleTiles = FindMiddleTiles(), targetData = {0, 0};
        for(int i = 0; i < middleTiles.Length; i++)
        {
            targetData[0] = i;
            targetData[1] = middleTiles[i];
            MoveCharacterHere(targetData);
            // MoveCharacterHere(i, middleTiles[i]);
        }   
    }

    int[] FindMiddleTiles()
    {
        int tileAmount = 0;
        int[] middleTiles = {0, 0};

        for(int i = 0; i < Tiles.Length; i++)
        {
            if(Tiles[i].gameObject.activeSelf)
                tileAmount++;
        }   
        if(tileAmount % 2 == 0)
        {
            // Debug.Log("Even amount of tiles");
            middleTiles[1] = (tileAmount / 2);
            middleTiles[0] = middleTiles[1] - 1;
        }
        else
        {
            // Debug.Log("Odd amount of tiles");
            middleTiles[1] = (int)Mathf.Ceil((tileAmount / 2)) + 1;
            middleTiles[0] = (int)Mathf.Floor((tileAmount / 2) - 1);
        }
        // Debug.Log("Player one will be on tile: " + middleTiles[0] + ". Player two will be on tile: " + middleTiles[1]);
        return middleTiles;
    }

    public void MoveCharacterHere(int[] targetData)
    {
        CharacterDisplay fighter = Fighters[targetData[0]];
        Tiles[fighter.CurrentTileID].SetOccupant(false);
        Tiles[targetData[1]].SetOccupant(true, fighter);
        fighter.CurrentTileID = targetData[1];
    }

    public bool IsTileOccupied(int targetTile)
    {
        return Tiles[targetTile].hasOccupant;
    }

    bool IsValidTileID(int targetTile)
    {
        bool isValidTile = (targetTile >= 0 & targetTile < Tiles.Length);
        // Debug.Log("targetTile " + targetTile + " is a valid tile: " + isValidTile);
        return isValidTile;
    }
}
