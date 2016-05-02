
/*
 __________________________________________________________________________________________________________________________

    These are the rules of the Sudoku Double Bonus
__________________________________________________________________________________________________________________________
   
        **Gameplay        
        The game begins by having one player place a tile from their hand into one of the game board cells
        in the center sector of the board. The next player must then play a tile that connects directly to a 
        tile on the board on one of its adjacent sides. This play cannot be played diagonally. Play continues
        like this until the center sector is completed. At this point the rest of the board opens up and the
        players may play in any sector they wish as long as there play connects on one of its adjacent sides 
        to a tile that is already on the board. 

        **Valid Play
        A valid play is any play that does not disobey the following rules.
        1. It must be the players turn
        2. Play must start in the center sector, and remain there until the center sector is complete.
        3. Every play must be made by connecting the played tile to another played tile on one of its adjacent sides, not diagonally.
           Except for the very first play on the board, since there are no other tiles to play adjacent to.
        4. There can be only one of any number between 1-9 in any Row
        5. There can be only one of any number between 1-9 in any Column
        6. There can be only one of any number between 1-9 in any sector

        **Scoring
        If a player completes a Row, Column or Sector that player receives 1 point for the face value of every tile in that 
        Row, Column or Sector. That is to say 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 = 45 points. If the player completes a row and a column
        the player receives 45 points for the row, and another 45 points for the column. If the player completes a row, a column and a sector
        that player receives 45 points for each. 
        If a Row, Column or Sector is completed by closing it out with a bonus tile the player scores twice the points they would normally 
        have received 90 points. If a player completes any combination of Row, Column or Sector on a bonus tile, they receive twice the points
        they would normally receive for each.

        The board can only be completed by placing tiles onto every cell on the board, when the board is completed, the player with the most points
        wins the game.

        Intended game modes
        - Player vs Computer (AI) --- Initial Scope
        - Player vs Player (Facebook) --- Extended Scope
        - Player vs Player (Local) --- Extended Scope
        - 3 and 4 player modes --- Extended Scope

        Intended Monetization
        - Player may purchase themed skins (Music, Summer etc.)
        - Player may purchase game modes like classic sudoku and time trial mode, 
        - Ads in the free version of the game that can be removed with full game purchase.

        *****ALTERNATIVE RULES*******
        Instead of winning by having the most points, the player with the least amount of points wins.

__________________________________________________________________________________________________________________________

           This is how the board is created
__________________________________________________________________________________________________________________________


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
          
          each cell is a TilePrefab that is instantiated upon board creation
          The only two possible tiles that can be created during board creation
          are the Bonus tile and the Blank tile
__________________________________________________________________________________________________________________________
          
          Checking to ensure the player has made a valid move
__________________________________________________________________________________________________________________________



       --- First check the row to make sure that the number they placed is not already in that row 
           ( using the y position of the tiles in that row, to get their value ) 

       --- Second check the column to make sure that the number they placed is not already in that column 
           ( using the x position of the tiles in that column to get their value ) 

       --- Third check the remaining cells in the sector that the tile was placed in to make sure 
           this sector does not already have that number

       if these checks all pass, then the tile can be placed at that location and play may resume.

       Highest possible x is 8, lowest is 0
       Highest possible y is 8, lowest is 0

       if the player attempted to place the tile at location K (H, 5)
       Cells marked with an A are the cells in the row that need to be checked 
       Cells marked with a B are the cells in the column that need to be checked.
       Cells marked with a C are checked with a separate method for clarity, which is 
       explained in more detail below.

       These checks will ensure the player is not attempting to play a tile on top of another tile
       they will also check to make sure that the player is playing adjacent (connected) to at least
       one other tile. Finally these checks will make sure that in the row and sector that is being 
       played in, the value of the new tile being played is not already present in the row or column.

       Checking A and B (Rows and Columns)

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

        Checking C (Sectors)

        After checking whether the tile can be placed in the chosen row and column one more check must be completed
        this check is the sector check.

        example of one such check
        The following example takes a sector that the player has chosen to play in
        in this sector there are four tiles already placed in the sector, the player is attempting to place
        a new tile from their hand into the sector labelled R
         
        1 | 5 | X   This method occurs after the row and column checks have already
        ---------   been completed so.   
        X | 7 | X   X = null, unplayed upon cell, no tile here
        ---------   If the player attempts to place a tile at the location of R
        X | 9 | R   R is also a null cell at this point, no tile here yet either

        1 | 5 | C   We can imagine the board looks like this to the game right now
        ---------   C = checked already, anything without a C needs to be checked
        X | 7 | C   This method will now check the top left, top center, center left, and center tiles
        ---------   if any of these (non null) tiles holds the same value as the tile the player wants to play
        C | C | R   the method returns false, since play is invalid. (CCR reference unintentional...)

        The sector check is the most detailed check in the process of validating a players move. It needs to determine whether
        any of the tiles in the sector that the new tile was played in have the same value as the new tile.
 */
