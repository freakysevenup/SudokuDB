using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

    //   public Vector2 touchBegan;
    //   public Vector2 touchEnded;

    //   public Vector2 touchCurrentPosition;

    //   public bool thereWasATouch;

    //   private void handleTouch()
    //   {
    //       // Get the location where the original touch began, this is ideally the tile the player wants to move
    //       // assign that location to the touchedTile Vector2
    //       if (Input.GetTouch(0).phase == TouchPhase.Began)
    //       {
    //           // Rounding allows for the choosing of tiles and the placement of tiles to snap into place
    //           touchBegan = new Vector2(Mathf.Round(Input.GetTouch(0).position.x), Mathf.Round(Input.GetTouch(0).position.y));
    //           thereWasATouch = true;
    //       }
    //       else
    //       {
    //           thereWasATouch = false;
    //       }

    //       // Get the current position of the touch on the screen everytime the finger has moved, and store that into a variable
    //       // that can be called upon by the player so that the tile can move across the screen exactly where the player is touching the screen
    //       if (Input.GetTouch(0).phase == TouchPhase.Moved)
    //       {
    //           //touchCurrentPosition = Input.GetTouch(0).position;//new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
    //           //Debug.Log("x: " + Input.GetTouch(0).position.x + " y: " + Input.GetTouch(0).position.y);
    //           thereWasATouch = true;
    //       }
    //       else
    //       {
    //           thereWasATouch = false;
    //       }

    //       // Get the location where the touch ended, this is ideally where the player wishes to place the tile
    //       // assigne that to the position vector2
    //       if (Input.GetTouch(0).phase == TouchPhase.Ended)
    //       {
    //           // Rounding allows for the choosing of tiles and the placement of tiles to snap into place
    //           touchEnded = new Vector2(Mathf.Round(Input.GetTouch(0).position.x), Mathf.Round(Input.GetTouch(0).position.y));
    //           thereWasATouch = true;
    //       }
    //       else
    //       {
    //           thereWasATouch = false;
    //       }
    //   }

    //void Start ()
    //   {
    //       touchEnded = new Vector2();
    //       touchBegan = new Vector2();
    //}

    //void Update ()
    //   {
    //       Debug.Log(Input.touchCount.ToString());
    //       if (Input.touchCount > 0)
    //       {
    //           handleTouch();
    //       }

    //       if (thereWasATouch)
    //       {
    //           touchCurrentPosition = Input.GetTouch(0).position;
    //       }
    //   }

    public bool mouseLeft;
    public bool mouseRight;
    public bool mouseMoved;
    public Vector2 mousePosition;

    void Start()
    {
        mouseLeft = false;
        mouseRight = false;
        mousePosition = new Vector2();
    }

    void checkMouseMotion()
    {
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        Vector2 previousPosition = mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector2(Mathf.Round(mouseX), Mathf.Round(mouseY)));

        if(mousePosition != previousPosition)
        {
            mouseMoved = true;
        }
        else
        {
            mouseMoved = false;
        }
    }

    void checkMouseButtons()
    {
        if(Input.GetButtonUp("Fire1"))
        {
            mouseLeft = false;
        }
        else if(Input.GetButtonDown("Fire1"))
        {
            mouseLeft = true;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            mouseRight = false;
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            mouseRight = true;
        }
    }

    void Update()
    {
        checkMouseButtons();
        checkMouseMotion();
    }



}
