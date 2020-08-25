using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public int cellSize;
    public float shiftDelay;

    public GameObject Orb;


    public bool isShifting { get; set; }

    public Grid<int> grid;
    public GameObject[,] orbGrid;

    public bool checkGridFlag = false;


    // Start is called before the first frame update
    void Start()
    {
        isShifting = false;

        grid = new Grid<int>(gridWidth, gridHeight, cellSize);

        orbGrid = new GameObject[gridWidth,gridHeight];

        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {  
                grid.SetValue(x, y, Random.Range(0,5));
                int gridIcon = grid.GetValue(x, y);
                GameObject newOrb = null;
                newOrb = Instantiate(Orb, new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f), new Quaternion(0f, 0f, 0f, 0f));
                newOrb.gameObject.name = ((OrbTypes)gridIcon).ToString();
                OrbBehavior orbScript = newOrb.GetComponent<OrbBehavior>();
                orbScript.OrbType = (OrbTypes)gridIcon;
                orbGrid[x, y] = newOrb;
                newOrb.transform.parent = transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void restartShifting()
    {
        StopAllCoroutines();
        StartCoroutine(FindNullTiles());
    }
    public void GetOrbGridXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.y / cellSize);
    }

    public void setOrbGridValue(int x,int y,GameObject orb)
    {
        orbGrid[x,y] = orb;
    }


    public IEnumerator FindNullTiles()
    {
        yield return new WaitForSeconds(0.1f);
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (orbGrid[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                orbGrid[x, y].GetComponent<OrbBehavior>().ClearAllMatches();
            }
        }
    }
    private IEnumerator ShiftTilesDown(int x, int yStart)
    {
        isShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;
        bool gap = false;
        for (int y = yStart; y < gridHeight; y++)
        {
            SpriteRenderer render = orbGrid[x, y].GetComponent<SpriteRenderer>();
            RaycastHit2D hit = Physics2D.Raycast(orbGrid[x,y].gameObject.transform.position, Vector2.up, 2.5f);
            
            if(nullCount!=0 && render.sprite != null)
            {
                gap = true;
            }
            if (render.sprite == null && !gap)
            {
                nullCount++;
                //Debug.DrawRay(orbGrid[x, y].gameObject.transform.position, Vector2.down, Color.black, 1f);
            }
            renders.Add(render);
        }

        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < renders.Count - 1; k++)
            {
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, gridHeight - 1);
            }
        }
        isShifting = false;
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> newOrbs = new List<Sprite>();
        List<Sprite> os = orbGrid[0, 0].gameObject.GetComponent<OrbBehavior>().getOrbs();
        newOrbs.AddRange(os);

        if (x > 0)
        {
            newOrbs.Remove(orbGrid[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < gridWidth - 1)
        {
            newOrbs.Remove(orbGrid[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            newOrbs.Remove(orbGrid[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return newOrbs[Random.Range(0, newOrbs.Count)];
    }

}
