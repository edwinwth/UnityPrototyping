using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    //Board size parameters
    public int gridWidth;
    public int gridHeight;
    public int cellSize;

    //Spawnable references
    public GameObject pickUps;
    public GameObject player;
    public List<GameObject> playerTurnIcon;
    public GameObject finishFlagIcon;

    //UI references
    public Text attackCounterText;
    public Text defendCounterText;
    public Text healCounterText;
    public Text stepCounterText;


    //script variables
    private int attackCounter = 0;
    private int defendCounter = 0;
    private int healCounter = 0;
    private PlayerBehavior playerScript;
    private List<float> turnMeters = new List<float>{ 0.0f, 0.0f, 0.0f, 0.0f };
    private List<Vector2> iconStartPos;
    private Vector2 finishFlagPos;
    private int teamTurnCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        initailizeIconList();
        finishFlagPos = finishFlagIcon.transform.position;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int pickUpValue = Random.Range(0, 3);
                if(x==gridWidth/2 && y == gridHeight / 2) //create Player in middle;
                {
                    GameObject playerObject = null;
                    playerObject = Instantiate(player, new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f), new Quaternion(0f, 0f, 0f, 0f));
                    playerScript = playerObject.GetComponent<PlayerBehavior>();
                    continue;
                }
                GameObject newPickUp = null;
                newPickUp = Instantiate(pickUps, new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f), new Quaternion(0f, 0f, 0f, 0f));
                PickUpBehavior pickUpScript = newPickUp.GetComponent<PickUpBehavior>();
                pickUpScript.setPickUpType((PickUpType)pickUpValue);
                pickUpScript.setSprite(pickUpValue);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateCounters();
        turnUpdate();
        moveTurnBar();
    }
    
    public void addCounter(PickUpType type) //Add to specific type counter, invoked by playerBehavior
    {
        switch (type)
        {
            case PickUpType.attack:
                attackCounter += 1;
                break;
            case PickUpType.defend:
                defendCounter += 1;
                break;
            case PickUpType.heal:
                healCounter += 1;
                break;
        }
    }

    public int getGridHeight()
    {
        return gridHeight;
    }
    public int getGridWidth()
    {
        return gridWidth;
    }

    private void initailizeIconList()
    {
        iconStartPos = new List<Vector2>(playerTurnIcon.Count);
        for (int i = 0; i < playerTurnIcon.Count; i++)
            iconStartPos.Add(playerTurnIcon[i].transform.position);
    }

    private void updateCounters()
    {
        //update counter debug text
        attackCounterText.text = attackCounter.ToString();
        defendCounterText.text = defendCounter.ToString();
        healCounterText.text = healCounter.ToString();
        stepCounterText.text = playerScript.stepCounter.ToString();
    }

    private void moveTurnBar()
    {
        for(int i = 0; i < playerTurnIcon.Count; i++)
        {
            playerTurnIcon[i].transform.position = Vector2.Lerp(iconStartPos[i], finishFlagPos, turnMeters[i] / 10.0f);
        }

    }
    private void turnUpdate()
    {
        if (playerScript.stepCounter <= 0 && !playerScript.getIsLerping())
        {
            for(int i = 0; i < turnMeters.Count; i++)
            {
                turnMeters[i] += playerScript.getSpeed(i);
                if (turnMeters[i] >= 10.0f)
                {
                    playerScript.newTurn(i);
                    GameObject[] listOfPickUp = GameObject.FindGameObjectsWithTag("PickUps");
                    foreach(GameObject pickUp in listOfPickUp)
                    {
                        Destroy(pickUp);
                    }
                    for (int x = 0; x < gridWidth; x++)
                    {
                        for (int y = 0; y < gridHeight; y++)
                        {
                            int pickUpValue = Random.Range(0, 3);
                            if (x == gridWidth / 2 && y == gridHeight / 2) //create Player in middle;
                            {
                                continue;
                            }
                            GameObject newPickUp = null;
                            newPickUp = Instantiate(pickUps, new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f), new Quaternion(0f, 0f, 0f, 0f));
                            PickUpBehavior pickUpScript = newPickUp.GetComponent<PickUpBehavior>();
                            pickUpScript.setPickUpType((PickUpType)pickUpValue);
                            pickUpScript.setSprite(pickUpValue);
                        }
                    }
                    attackCounter = 0;
                    defendCounter = 0;
                    healCounter = 0;
                    turnMeters[i] = 0.0f;
                }
            }
        }
    }
}
