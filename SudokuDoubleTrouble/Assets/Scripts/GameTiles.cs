using UnityEngine;
using System.Collections.Generic;

public class GameTiles : MonoBehaviour
{
    const int TOTAL_AMOUNT_OF_TILES = 81;
    const int HAND_SIZE = 7;
    const float TILE_SIZE = 0.35f;
    static byte counter = 0;
    Tile[] tile;
    public Tile[] playerHand;
    Tile thisTile;
    static Dictionary<int, Tile> bag = new Dictionary<int, Tile>();     // Map of Tiles available in the game
    static int[] numbersChosen;

    protected void initialize()
    {
        counter = 0;
        tile = new Tile[TOTAL_AMOUNT_OF_TILES];
        playerHand = new Tile[HAND_SIZE];
        numbersChosen = new int[TOTAL_AMOUNT_OF_TILES];

        for ( int i = 0; i < TOTAL_AMOUNT_OF_TILES; i++)
        {
            numbersChosen[i] = 100;
        }

        fillBag();
    }

    private void fillBag()
    {
        if (counter < TOTAL_AMOUNT_OF_TILES)
        {
            // For the 9 different tiles available
            for (byte i = 1; i < 10; i++)
            {
                for(byte j = 0; j < 9; j++)
                {
                    counter++;
                    // Create a new tile
                    // set the value of the tile
                    // set the Id of the tile to a unique id value
                    // set position of the tile to zero
                    // set the state of the tile to In the bag
                    tile[counter - 1] = new Tile();
                    tile[counter - 1].setTileVariables(Tile.State.IN_BAG, Vector2.zero, i, counter);
                    // Store the newly created tile and the value of that tile in the bag
                    bag.Add(counter - 1, tile[counter - 1]);                  
                }
            }
        }

        // if the bag is now full of tiles print out the size of the bag
        ////Debug.Log("Success, the bag has " + bag.Count + " tiles in it.");
        ////for(int i = 0; i < bag.Count; i++)
        ////{
        ////    Debug.Log(bag[i].ID);
        ////    Debug.Log(bag[i].TileValue);
        ////}
    }

    public Tile pullFromBag()
    {
        
        Tile temp = new Tile();
        // Try to get a tile from the bag 
        // Choose a random number between 1 and 9
        int chosenTile = Random.Range(1, bag.Count);
        Debug.Log(chosenTile);

        // for every tile chosen from the bag
        // if that number has already been chosen, chose again.
        // come back and check the number again, do this check up to seven times
        // if that is needed.
        for (int i = 0; i < HAND_SIZE; i++)
        {
            // if a double was found choose a new number
            // and print to the log that a double was found
            if (checkForDouble(chosenTile) == true)
            {
                chosenTile = Random.Range(1, bag.Count);
                Debug.Log("found a double " + chosenTile);
            }
        else
        {
             // otherwise if no double was found
             // stop the loop without breaking the loop 
             // conventionally
            i = HAND_SIZE - 1;
        }
    }

        // return the tile and make its place in the bag be null
        if (bag.Count > 0)
        {
            temp = bag[chosenTile];
            bag.Remove(chosenTile);
            bag[chosenTile] = null;
        }

        //Debug.Log(bag.Count);

        return temp;
    }

    private bool checkForDouble(int chosenNumber)
    {
        bool returnValue = false;
        // if the list of numbers chosen doesn't contain the chosen number
        if (numbersChosen[chosenNumber] == 100)
        {
            // make the place in the list of chosen numbers be filled with that number
            numbersChosen[chosenNumber] = chosenNumber;
            returnValue = false;
        }
        // otherwise if the number chosen is already in the list 
        else if(numbersChosen[chosenNumber] == chosenNumber)
        {
            returnValue = true;
        }
        return returnValue;
    }

    public Tile[] exchangeTiles(Tile[] tiles)
    {
        // You must return your tiles to the bag before puling new tiles
        for(int i = 0; i < tiles.Length; i++)
        {
            // If the bag is not full you may add tiles to the bag
            // Should never be a problem, but I figured I'd just check
            if (bag.Count < TOTAL_AMOUNT_OF_TILES)
            {
                bag.Add(bag.Count + 1, tiles[i]);
            }
            else
            {
                Debug.Log("The Bag of Tiles is full it is impossible to exchange tiles at this time");
            }
        }

        // Pull new tiles from the bag
        // You may only pull as many tiles from the bag as you put into it
        Tile[] newTiles = new Tile[tiles.Length];
        for(int i = 0; i < newTiles.Length; i++)
        {
            newTiles[i] = pullFromBag();
        }

        return newTiles;
    }

    void Start ()
    {
        initialize();
	}
	
	void Update () {
	
	}
}
