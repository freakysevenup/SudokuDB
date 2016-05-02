using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour {

    // Self explanatory constants, (NO MAGIC NUMBERS)
    const int TOTAL_AMOUNT_OF_TILES = 81;
    const float CELL_SIZE = 0.35f;
    const int ROW_WIDTH_PLUS_ONE = 9;
    const int COLUMN_HEIGHT_PLUS_ONE = 9;

    public Dictionary<Vector2, Transform> gameBoard = new Dictionary<Vector2, Transform>();                //The GameBoard
    public Dictionary<Vector2, Tile> tilesOnBoard = new Dictionary<Vector2, Tile>();                       //The placed Tiles
    public Dictionary<Vector2, bool> gameBoardMap = new Dictionary<Vector2, bool>();                       //Places that can be played on during the game
    public Dictionary<Vector2, bool> canBePlayedOn = new Dictionary<Vector2, bool>();                      //Places that can be played on in the beginning of the game

    bool middleSectorFilled = false;                                                                // Middle sector must be filled first in order to play on
    bool matchInRow = false;                                                                        // Was there a match in the row
    bool matchInColumn = false;                                                                     // Was there a match in the column
    bool matchInSector = false;                                                                     // Was there a match in the sector

    Transform blankTile;                                                                            // The transform of the blank cells that make up the GameBoard
    Transform bonusTile;                                                                            // The transform of the bonus cells that make up the GameBoard

    private void makeBoard()
    {
        /*
        This is how the board is created from cell 1 to cell 81
        Cells marked with a B are bonus cells
        The Board is arranged with 9 Sectors of 9 cells

          73 74 75 | B  77 78 | 79 80 81
          64 B  66 | 67 68 69 | 70 71 72
          55 56 57 | 58 59 60 | 61 62 B
          -------- | -------- | --------
          46 47 48 | 49 B  51 | 52 53 54
          B  38 39 | 40 41 42 | 43 44 45
          28 29 30 | 31 32 33 | 34 B  36
          -------- | -------- | --------
          19 29 21 | 22 23 B  | 25 26 27
          10 11 B  | 13 14 15 | 16 17 18
          1  2  3  | 4  5  6  | B  8  9

        */

        int cellCounter = 0;
        // For each of 9 rows
        // and each of 9 columns
        // draw a corresponding cell
        // to create the board.
        for (int y = 0; y < COLUMN_HEIGHT_PLUS_ONE; y++)
        {
            for (int x = 0; x < ROW_WIDTH_PLUS_ONE; ++x)
            {
                cellCounter++;
                Vector2 pos = new Vector2(x, y);
                if (   cellCounter == 7
                    || cellCounter == 12
                    || cellCounter == 24
                    || cellCounter == 35
                    || cellCounter == 37
                    || cellCounter == 50
                    || cellCounter == 63
                    || cellCounter == 65
                    || cellCounter == 76)
                {
                    // create a bonus cell
                    bonusTile = Instantiate(setBonusCell(), pos, Quaternion.identity) as Transform;
                    bonusTile.transform.localScale = new Vector2(CELL_SIZE, CELL_SIZE);
                    gameBoard.Add(pos, bonusTile);
                }
                else
                {
                    // create a blank cell
                    blankTile = Instantiate(setBlankCell(), pos, Quaternion.identity) as Transform;
                    blankTile.transform.localScale = new Vector2(CELL_SIZE, CELL_SIZE);
                    gameBoard.Add(pos, blankTile);
                }

                // Set the middle sector as the mandatory first play area
                if(    cellCounter == 31
                    || cellCounter == 32
                    || cellCounter == 33
                    || cellCounter == 40
                    || cellCounter == 41
                    || cellCounter == 42
                    || cellCounter == 49
                    || cellCounter == 50
                    || cellCounter == 51)
                {
                    // only the middle can be played on at first
                    // once it is full the play may move on to the rest of the board.
                    canBePlayedOn.Add(pos, true);
                }
                else
                {
                    // Set the initial places the players may play on to be only the middle sector
                    canBePlayedOn.Add(pos, false);
                }
                // Set that particular location to be unplayed on
                gameBoardMap.Add(pos, false);
                tilesOnBoard[pos] = null;
                //nullTile.setTileVariables(Tile.State.NULL, pos, 0, 0);
                //tilesOnBoard[pos] = nullTile;
            }
        }
        Debug.Log("Board was filled");
    }

    /*
        IF THIS METHOD RETURNS TRUE, THE PLAYERS PLAY WAS VALIDATED

        -- Calls upon the validateValue() method, which uses the 
            tile and the intended position to determine if the value 
            the player is attempting to play, violates any of the rules of the board.
            if the value is already in that row, column or sector the play is invalid.
            which will nullify this method, making it return false.

       This method still needs to check against the value of the tile being placed
       to ensure that no other tile in that row, column or sector has the same value
       It also needs to check to make sure there are no other tiles in the location the tile
       may be placed, also must make sure that there is a tile in at least one of the cardinal
       directions otherwise the play is invalid.

       So after checking that the cell is unoccupied and that there is at least one 
       tile in one of the cardinal directions, perform these checks

       if the player attempted to place the tile at location K (H, 5)

       --- First check the row to make sure that the number they placed is not already in that row 
           ( using the y position of the tiles in that row, to get their value ) 

       --- Second check the column to make sure that the number they placed is not already in that column 
           ( using the x position of the tiles in that column to get their value ) 

       --- Third check the remaining cells in the sector that the tile was placed in to make sure 
           this sector does not already have that number

       if these checks all pass, then the tile can be placed at that location and play may resume.

       Highest possible x is 8, lowest is 0
       Highest possible y is 8, lowest is 0

             A B C  D E F  G H I
          1  X|X|X  X|X|X  X|B|X
          2  X|X|X  X|X|X  X|B|X
          3  X|X|X  X|X|X  X|B|X
             -----  -----  -----
          4  X|X|X  X|X|X  C|B|C
          5  A|A|A  A|A|A  A|K|A   --A
          6  X|X|X  X|X|X  C|B|C
             -----  -----  -----
          7  X|X|X  X|X|X  X|B|X
          8  X|X|X  X|X|X  X|B|X
          9  X|X|X  X|X|X  X|B|X

                             |
                             B
       */
    public bool validatePlay(Vector2 pos, Tile tile)
    {
        //must remember to round the values of position before checking with position
        // this will ensure that the tile snaps to the closest possible cell
        Vector2 position = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        Tile tempTile;
        tilesOnBoard.TryGetValue(position, out tempTile);

        bool youMayPlayHere = false;
        // if the middle has not been filled yet
        if(!middleSectorFilled)
        {
            // Check to see where the player is trying to place the tile
            // if the tile is not inside the middle sector, then the player
            // cannot play here so method returns false;
            if(canBePlayedOn.TryGetValue(position, out youMayPlayHere))
            {
                    canBePlayedOn[position] = false;        // update the map of playable locations in the middle
                    gameBoardMap[position] = false;         // update the map of playable locations
                    gameBoard[position] = tile.TilePrefab;  // update the gameboard with the new tile

                // First find out if the tile can be placed there based on its value

                if(validateValue(position, tile))
                {
                    tilesOnBoard[position] = tile;          // update the placed tiles map with the new tile
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        // if the middle sector has been filled in
        if(middleSectorFilled)
        {
            // Check to see if the tile is currently occupied
            // if it is you cannot play here sp the method returns false,
            // otherwise it returns true.
            if(gameBoardMap.TryGetValue(position, out youMayPlayHere))
            {
                // Now need to check that the place the player is trying to place the tile
                // is connected to another tile on one of the four cardinal directions
                // if so the player may play here, otherwise they may not.
                bool thereIsATile = false;
                if (gameBoardMap.TryGetValue(position + new Vector2(0, 1), out thereIsATile)) // if there is a tile above the players chosen place
                {
                    if (thereIsATile)
                    {
                        if(validateValue(position, tile))                       // check that the value of the tile the player is placing is not already in the row, column or sector
                        {
                            gameBoardMap[position] = false;                     // update the map of playable locations
                            gameBoard[position] = tile.TilePrefab;              // update the gameboard with the new tile
                            return true;                                        // return true since they can play there
                        }
                        else
                        {
                            return false;                                       // if there is a tile in the row, column or sector that has this value the play is invalid
                        }
                    }
                }
                if (gameBoardMap.TryGetValue(position + new Vector2(0, -1), out thereIsATile)) // if there is a tile below the players chosen place
                {
                    if (thereIsATile)
                    {
                        if (validateValue(position, tile))                       // check that the value of the tile the player is placing is not already in the row, column or sector
                        {
                            gameBoardMap[position] = false;                     // update the map of playable locations
                            gameBoard[position] = tile.TilePrefab;              // update the gameboard with the new tile
                            return true;                                        // return true since they can play there
                        }
                        else
                        {
                            return false;                                       // if there is a tile in the row, column or sector that has this value the play is invalid
                        }
                    }
                }
                if (gameBoardMap.TryGetValue(position + new Vector2(1, 0), out thereIsATile)) // if there is a tile right of the players chosen place
                {
                    if (thereIsATile)
                    {
                        if (validateValue(position, tile))                       // check that the value of the tile the player is placing is not already in the row, column or sector
                        {
                            gameBoardMap[position] = false;                     // update the map of playable locations
                            gameBoard[position] = tile.TilePrefab;              // update the gameboard with the new tile
                            return true;                                        // return true since they can play there
                        }
                        else
                        {
                            return false;                                       // if there is a tile in the row, column or sector that has this value the play is invalid
                        }
                    }
                }
                if (gameBoardMap.TryGetValue(position + new Vector2(-1, 0), out thereIsATile)) // if there is a tile left of the players chosen place
                {
                    if (thereIsATile)
                    {
                        if (validateValue(position, tile))                       // check that the value of the tile the player is placing is not already in the row, column or sector
                        {
                            gameBoardMap[position] = false;                     // update the map of playable locations
                            gameBoard[position] = tile.TilePrefab;              // update the gameboard with the new tile
                            return true;                                        // return true since they can play there
                        }
                        else
                        {
                            return false;                                       // if there is a tile in the row, column or sector that has this value the play is invalid
                        }
                    }
                }
                else  // if there is no tile at any of the four cardinal positions around the players chosen place return false since they can't play there.
                {
                    return false;
                }
            }
            else  // there is already a tile in this location, so the player may not play here
            {
                return false;
            }
        }
        // Only here because C# is a crybaby
        return youMayPlayHere;
    }

    /*
        Determines if the value the player is attempting to play is a valid play
        according to the rules. 

        -- Calls upon the checkSectors() method, which takes the x and y values of the 
            posiitons of each tile in the tilesOnBoard Dictionary and checks the sector 
            that the player is attempting to play in to make sure that it doesn't already
            contain the value the player is attempting to play.

    */
    private bool validateValue(Vector2 position, Tile tile)
    {
        bool returnValue = false;

        int XVal = (int)(position.x);
        int YVal = (int)(position.y);
        Tile tempTile = new Tile();

        // Check the X axis
        for (int x = 0; x < ROW_WIDTH_PLUS_ONE; x++)
        {
            if (tilesOnBoard[new Vector2(x, YVal)] != null)
            {
                if (new Vector2(x, YVal) != position)
                {
                    tempTile = tilesOnBoard[position];

                    if (tempTile.TileValue == tile.TileValue)
                    {
                        returnValue = false;
                        matchInRow = true;
                        return returnValue;
                    }
                    else
                    {
                        matchInRow = false;
                    }
                }
            }

        }

        // check the y axis
        for (int y = 0; y < COLUMN_HEIGHT_PLUS_ONE; y++)
        {
            if (tilesOnBoard[new Vector2(XVal, y)] != null)
            {
                if (new Vector2(XVal, y) != position)
                {
                    tempTile = tilesOnBoard[position];

                    if (tempTile.TileValue == tile.TileValue)
                    {
                        returnValue = false;
                        matchInRow = true;
                        return returnValue;
                    }
                    else
                    {
                        matchInRow = false;
                    }
                }
            }

        }

        // check the surrounding tiles
        returnValue = checkSectors(XVal, YVal, tile);


        if(!matchInRow && !matchInColumn && !matchInSector)
        {
            returnValue = true;
        }
        return returnValue;
    }

    /*
        Will check the sector the player is attempting to play in to
        make sure that the value of the tile they are attempting to play
        is a valid play. if it is not a valid play the method returns false.

        example of one such check
         
         
        1 | 5 | X   This method occurs after the row and column checks have already
        ---------   been completed so.   
        X | 7 | X   X = null, unplayed upon cell, no tile here
        ---------   If the player attempts to place a tile at the location of R
        X | 9 | R   

        1 | 5 | C   We can imagine the board looks like this to the game right now
        ---------   C = checked already, anything without a C needs to be checked
        X | 7 | C   This method will now check the top left, top center, center left, and center tiles
        ---------   if any of these (non null) tiles holds the same value as the tile the player wants to play
        C | C | R   the method returns false, since play is invalid. (CCR reference unintentional...)

    */
    private bool checkSectors(int XVal, int YVal, Tile tile)
    {
        bool returnValue = false;

        // Sector Row 1
        if (XVal == 0 && YVal == 0 || XVal == 3 && YVal == 0 || XVal == 6 && YVal == 0)
        {
            // top center , top right, center right, center tile
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 1 && YVal == 0 || XVal == 4 && YVal == 0 || XVal == 7 && YVal == 0)
        {
            // top left, center left, top right, center right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 2 && YVal == 0 || XVal == 5 && YVal == 0 || XVal == 8 && YVal == 0)
        {
            // top left, center left, top center, center
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal + 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }


        else if (XVal == 0 && YVal == 1 || XVal == 3 && YVal == 1 || XVal == 6 && YVal == 1)
        {
            // top center, top right, bottom center, bottom right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal - 1)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 1 && YVal == 1 || XVal == 4 && YVal == 1 || XVal == 7 && YVal == 1)
        {
            //top right, top  left, bottom right, bottom left
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 1)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 2 && YVal == 1 || XVal == 5 && YVal == 1 || XVal == 8 && YVal == 1)
        {
            // top left, top center, bottom left, bottom center
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal - 1)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }


        else if (XVal == 0 && YVal == 2 || XVal == 3 && YVal == 2 || XVal == 6 && YVal == 2)
        {
            // bottom right, bottom center, center right, center
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal - 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 1 && YVal == 2 || XVal == 4 && YVal == 2 || XVal == 7 && YVal == 2)
        {
            // bottom left, bottom right, center left, center right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 2 && YVal == 2 || XVal == 5 && YVal == 2 || XVal == 8 && YVal == 2)
        {
            // center left, center, bottom left, center bottom
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal - 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }

        // Sector Row 2
        else if (XVal == 0 && YVal == 3 || XVal == 3 && YVal == 3 || XVal == 6 && YVal == 3)
        {
            // top center, top right, center, center right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 1 && YVal == 3 || XVal == 4 && YVal == 3 || XVal == 7 && YVal == 3)
        {
            // top left, center left, top right, center right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 2 && YVal == 3 || XVal == 5 && YVal == 3 || XVal == 8 && YVal == 3)
        {
            // top left, center left, top center, center
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal + 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }


        else if (XVal == 0 && YVal == 4 || XVal == 3 && YVal == 4 || XVal == 6 && YVal == 4)
        {
            // top center, top right, bottom center, bottom right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal - 1)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 1 && YVal == 4 || XVal == 4 && YVal == 4 || XVal == 7 && YVal == 4)
        {
            //top right, top  left, bottom right, bottom left
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 1)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 2 && YVal == 4 || XVal == 5 && YVal == 4 || XVal == 8 && YVal == 4)
        {
            // top left, top center, bottom left, bottom center
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal - 1)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }


        else if (XVal == 0 && YVal == 5 || XVal == 3 && YVal == 5 || XVal == 6 && YVal == 5)
        {
            // bottom right, bottom center, center right, center
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal - 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 1 && YVal == 5 || XVal == 4 && YVal == 5 || XVal == 7 && YVal == 5)
        {
            // bottom left, bottom right, center left, center right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 2 && YVal == 5 || XVal == 5 && YVal == 5 || XVal == 8 && YVal == 5)
        {
            // center left, center, bottom left, center bottom
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal - 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }

        // Sector Row 3
        else if (XVal == 0 && YVal == 6 || XVal == 3 && YVal == 6 || XVal == 6 && YVal == 6)
        {
            // top center, top right, center, center right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 1 && YVal == 6 || XVal == 4 && YVal == 6 || XVal == 7 && YVal == 6)
        {
            // top left, center left, top right, center right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 2 && YVal == 6 || XVal == 5 && YVal == 6 || XVal == 8 && YVal == 6)
        {
            // top left, center left, top center, center
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal + 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }


        else if (XVal == 0 && YVal == 7 || XVal == 3 && YVal == 7 || XVal == 6 && YVal == 7)
        {
            // top center, top right, bottom center, bottom right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal - 1)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 1 && YVal == 7 || XVal == 4 && YVal == 7 || XVal == 7 && YVal == 7)
        {
            //top right, top  left, bottom right, bottom left
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal + 1)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 2 && YVal == 7 || XVal == 5 && YVal == 7 || XVal == 8 && YVal == 7)
        {
            // top left, top center, bottom left, bottom center
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal + 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal - 1)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }


        else if (XVal == 0 && YVal == 8 || XVal == 3 && YVal == 8 || XVal == 6 && YVal == 8)
        {
            // bottom right, bottom center, center right, center
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 2, YVal - 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 1 && YVal == 8 || XVal == 4 && YVal == 8 || XVal == 7 && YVal == 8)
        {
            // bottom left, bottom right, center left, center right
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal + 1, YVal - 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        else if (XVal == 2 && YVal == 8 || XVal == 5 && YVal == 8 || XVal == 8 && YVal == 8)
        {
            // center left, center, bottom left, center bottom
            if (
                   tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 1, YVal - 2)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal - 1)].TileValue
                || tile.TileValue == tilesOnBoard[new Vector2(XVal - 2, YVal - 2)].TileValue)
            {
                matchInSector = true;
                returnValue = false;
                return returnValue;
            }
        }
        return returnValue;
    }

    private Transform setBlankCell()
    {
        return blankTile = Resources.Load<Transform>("BlankCell");
    }

    private Transform setBonusCell()
    {
        return bonusTile = Resources.Load<Transform>("DBCell");
    }

    void Start ()
    {
        makeBoard();
	}
	
	void Update ()
    {
        // Check to see whether or not the 
        // middle sector has been filled in yet
        // only if it hasn't already been filled in
        if(!middleSectorFilled)
        {
            for (int i = 0; i < canBePlayedOn.Count; i++)
            {
                if (!canBePlayedOn.ContainsValue(true))
                {
                    middleSectorFilled = true;
                }
            }
        }
	}
}
