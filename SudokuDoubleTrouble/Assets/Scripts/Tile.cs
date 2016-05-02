using UnityEngine;
using System.Collections;

public class Tile {

    public enum State
    {
        PLACED = 0,
        IN_PLAYER_HAND,
        IN_BAG,
        NULL
    }

    private byte id;                            // Tile Identifier
    private byte tileValue;                     // Tile Value
    private Vector2 position;                   // Tile Position
    private State currentState;                 // Tile State
    private Transform tilePrefab;               // Tile Prefab
    public bool updatePosition = false;         // Can the tile be moved
        
	void Start ()
    {
        initialize();
    }

    // Initialize Tile Variables
    void initialize()
    {
        ID = 0;
        TileValue = 0;
        Position = Vector2.zero;
        CurrentState = State.IN_BAG;
    }

    //Properties
    public byte ID
    {
        get { return id; }
        set
        {
            if(value != id)
            {
                id = value;
            }
        }
    }
    public byte TileValue
    {
        get { return tileValue; }
        set
        {
            if(value != tileValue)
            {
                tileValue = value;
            }
        }
    }
    public Vector2 Position
    {
        get { return position; }
        set
        {
            if(value.x != position.x)
            {
                position.x = value.x;
            }
            if (value.y != position.y)
            {
                position.y = value.y;
            }
        }
    }
    public State CurrentState
    {
        get { return currentState; }
        set
        {
            if(value != currentState)
            {
                currentState = value;
            }
        }
    }
    public Transform TilePrefab
    {
        get { return tilePrefab; }
        set
        {
            if (value != tilePrefab)
            {
                tilePrefab = value;
            }
        }
    }
    protected void setPrefab(byte prefab)
    {
        switch(prefab)
        {
            case 1:
                TilePrefab = Resources.Load<Transform>("1_Tile 1");
                break;
            case 2:
                TilePrefab = Resources.Load<Transform>("2_Tile 1");
                break;
            case 3:
                TilePrefab = Resources.Load<Transform>("3_Tile 1");
                break;
            case 4:
                TilePrefab = Resources.Load<Transform>("4_Tile 1");
                break;
            case 5:
                TilePrefab = Resources.Load<Transform>("5_Tile 1");
                break;
            case 6:
                TilePrefab = Resources.Load<Transform>("6_Tile 1");
                break;
            case 7:
                TilePrefab = Resources.Load<Transform>("7_Tile 1");
                break;
            case 8:
                TilePrefab = Resources.Load<Transform>("8_Tile 1");
                break;
            case 9:
                TilePrefab = Resources.Load<Transform>("9_Tile 1");
                break;
            default:
                break;
        }
    }
    public void setTileVariables(State pState, Vector2 pPosition, byte pValue, byte pId)
    {
        CurrentState = pState;
        Position = pPosition;
        TileValue = pValue;
        ID = pId;

        setPrefab(pValue);
    }

	void Update ()
    {
	
	}
}
