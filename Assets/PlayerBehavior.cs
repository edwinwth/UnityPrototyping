using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public int stepCounter;
    public float timeForLerp;
    public List<Sprite> teamSprites;
    public List<float> playerSpeed;
    
    private Vector3 mousePosition;
    private bool clickedOn;
    private bool isLerping;
    private string lerpDirection;
    private Vector2 lerpStartPos;
    private Vector2 lerpEndPos;
    private Vector2 startPos; //i.e. middle of board
    private float timeStarted;
    private BoardManager boardScript;
    private int[] playerPos = new int[2] { 0, 0 };
    private int gridWidth;
    private int gridHeight;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        boardScript = GameObject.Find("BoardManager").GetComponent<BoardManager>();
        gridWidth = boardScript.getGridWidth();
        gridHeight = boardScript.getGridHeight();
        playerPos[0] = gridWidth / 2;
        playerPos[1] = gridHeight / 2;
    }

    // Update is called once per frame
    void Update()
    {
        getInput();
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStarted = Time.time - timeStarted;
            float lerpPercentage = timeSinceStarted / timeForLerp;
            transform.position = Vector2.Lerp(lerpStartPos, lerpEndPos, lerpPercentage);
            if(lerpPercentage >= 1f)
            {
                isLerping = false;
                lerpDirection = null;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) //Player Touches Anything
    {

        GameObject hit = collision.gameObject;
        if (hit.tag == "PickUps") //Plyaer touches a PickUp
        {
            PickUpBehavior hitScript = hit.GetComponent<PickUpBehavior>();
            PickUpType hitType = hitScript.getPickUpType();
            boardScript.addCounter(hitType); //add to counter on BoardManager Script;
            Destroy(hit);
        }
    }
    
    private void startLerp() //set up lerp parameters
    {
        lerpStartPos = transform.position;
        timeStarted = Time.time;
        switch (lerpDirection)
        {
            case "Left":
                if (playerPos[0] <= 0)
                    return;
                playerPos[0] -= 1;
                lerpEndPos = lerpStartPos - new Vector2(1f, 0f);
                break;
            case "Right":
                if (playerPos[0] >= gridWidth - 1)
                    return;
                playerPos[0] += 1;
                lerpEndPos = lerpStartPos + new Vector2(1f, 0f);
                break;
            case "Up":
                if (playerPos[1] >= gridHeight - 1)
                    return;
                playerPos[1] += 1;
                lerpEndPos = lerpStartPos + new Vector2(0f, 1f);
                break;
            case "Down":
                if (playerPos[1] <= 0)
                    return;
                playerPos[1] -= 1;
                lerpEndPos = lerpStartPos - new Vector2(0f, 1f);
                break;
            case null:
                return;
        }
        isLerping = true;
        stepCounter -= 1;
    }

    private void getInput() //Input listerner
    {
        if (isLerping || stepCounter <=0)
            return;
        if (Input.GetButtonUp("Left"))
        {
            lerpDirection = "Left";
            startLerp();
        }
        if (Input.GetButtonUp("Right"))
        {
            lerpDirection = "Right";
            startLerp();
        }
        if (Input.GetButtonUp("Up"))
        {
            lerpDirection = "Up";
            startLerp();
        }
        if (Input.GetButtonUp("Down"))
        {
            lerpDirection = "Down";
            startLerp();
        }
    }

    public bool getIsLerping()
    {
        return isLerping;
    }

    public void newTurn(int teamMemberID)
    {
        playerPos[0] = gridWidth / 2;
        playerPos[1] = gridHeight / 2;
        stepCounter = 5;
        transform.position = startPos;
        GetComponent<SpriteRenderer>().sprite = teamSprites[teamMemberID];
    }

    public float getSpeed(int teamMemberID)
    {
        return playerSpeed[teamMemberID];
    }
}
