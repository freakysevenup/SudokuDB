using UnityEngine;

public class Player : MonoBehaviour
{

    public InputHandler input;
    public GameTiles gTiles;
    public Board board;
    const float TILE_SIZE = 0.35f;
    const int HAND_SIZE = 7;

    public Tile[] playerHand;
    public int tilesInHand;
    public int score;

    //bool updatePosition;

    Tile[] exchangeQueue;

    void Awake()
    {
        board = GameObject.Find("GameSetup").GetComponent<Board>();
        gTiles = GameObject.Find("GameSetup").GetComponent<GameTiles>();
        input = GameObject.Find("GameSetup").GetComponent<InputHandler>();
        score = 0;
        tilesInHand = HAND_SIZE;
        playerHand = new Tile[HAND_SIZE];
    }

    // Use this for initialization
    void Start()
    {
        setPlayerHand();
    }

    public void playerInput()
    {
        /*

        Each play must be validated with the board, validation checks to make sure that the 
        play in question is not in conflict with any of the rules of the game.

        must play the middle sector to completion first
        only 1 - 9 in a sector
        only 1 - 9 in a row
        only 1 - 9 in a column
        must be your turn to play

        Need to update these from the game board for each tile that is validated

        Dictionary<Vector2, Transform> gameBoard = new Dictionary<Vector2, Transform>();                //The GameBoard
        Dictionary<Vector2, Tile> tilesOnBoard = new Dictionary<Vector2, Tile>();                       //The placed Tiles
        Dictionary<Vector2, bool> gameBoardMap = new Dictionary<Vector2, bool>();                       //Places that can be played on during the game
        Dictionary<Vector2, bool> canBePlayedOn = new Dictionary<Vector2, bool>();                      //Places that can be played on in the beginning of the game


        This is where all the magic happens for gameplay
        I have to really sit down with this method and 
        reason out what needs to happen to make this work right

         */


        Vector2 origPos = new Vector2();
        // determine what tile was selected
        for(int i = 0; i < tilesInHand; i++)
        {
            if (input.mouseLeft)
            {
                if (Mathf.Round(input.mousePosition.x) == playerHand[i].Position.x
                    && Mathf.Round(input.mousePosition.y) == playerHand[i].Position.y)
                {
                    playerHand[i].updatePosition = true;
                    origPos = playerHand[i].Position;
                }
            }
            else
            {
                playerHand[i].updatePosition = false;
            }

            // take the selected tile and move it with the mouse
            if(playerHand[i].updatePosition)
            {
                playerHand[i].TilePrefab.position = input.mousePosition;
                playerHand[i].Position = input.mousePosition;

                // if the tile was placed in a valid location
                // the tile state is now placed, otherwise 
                // if the tile was not placed in a valid location
                // the tile returns to the players hand.
                if (board.validatePlay(playerHand[i].Position, playerHand[i]))
                {
                    Debug.Log("The Play Was Validated");
                    playerHand[i].updatePosition = false;
                    playerHand[i].CurrentState = Tile.State.PLACED;
                    board.gameBoard[playerHand[i].Position] = playerHand[i].TilePrefab;
                    playerHand[i] = null;
                    playerHand[i] = gTiles.pullFromBag();
                    // if the play was valid, the player needs to get a new tile from the bag

                }
                else
                {
                    Debug.Log("The Play Was Not Validated");
                    if (playerHand[i].updatePosition)
                    {
                        //playerHand[i].Position = origPos;
                    }
                }
            }
        }
    }

    // Add single Tiles to the exchangeQueue
    public void addToExchangeQueue(Tile tile)
    {
        // Create a new Tile array that is bigger than the exchange wueue by one
        Tile[] temp = new Tile[exchangeQueue.Length + 1];

        // transfer all values from the exchangeQueue into the new tile Array
        for (int i = 0; i < exchangeQueue.Length; i++)
        {
            temp[i] = exchangeQueue[i];
        }

        // Add the new Tile to the temp Tile Array
        temp[exchangeQueue.Length + 1] = tile;

        // Make the exchangeQueue equate to the temp tile Array
        exchangeQueue = temp;
    }

    // Setters
    private Tile[] setPlayerHand()
    {
        // At the beginning of a game fill a players hand with 7 new Tiles
        // From the bag of tiles, set the variables for the Tile
        for (int i = 0; i < HAND_SIZE; i++)
        {
            playerHand[i] = gTiles.pullFromBag();
            playerHand[i].Position = new Vector2(-8 + i, 2);
            playerHand[i].CurrentState = Tile.State.IN_PLAYER_HAND;
            playerHand[i].TilePrefab = Instantiate(playerHand[i].TilePrefab, playerHand[i].Position, Quaternion.identity) as Transform;
            playerHand[i].TilePrefab.transform.localScale = new Vector2(TILE_SIZE, TILE_SIZE);
        }

        return playerHand;
    }

    // Getters
    public Tile[] getPlayerHand() { return playerHand; }
    public Tile[] getExchangeTiles() { return exchangeQueue; }


    // Update is called once per frame
    void Update()
    {
        playerInput();
    }
}
