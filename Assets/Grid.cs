using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;
    private bool debugBool = true;

    public Grid(int width,int height,float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];

        if (debugBool)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    //debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize * 0.5f, cellSize * 0.5f,0), 20, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }   

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y, 0) * cellSize;
    }
    
    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.y / cellSize);
    }

    public void SetValue(int x, int y, TGridObject value)
    {
        if(x >=0 && y >=0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            //debugTextArray[x, y].text = gridArray[x, y]?.ToString();
        }
    }


    public void SetValue(Vector3 worldPosistion, TGridObject value)
    {
        int x, y;
        GetXY(worldPosistion, out x, out y);
        SetValue(x, y, value);
    }

    public TGridObject GetValue(int x,int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        } else
        {
            return default(TGridObject);
        }
    }
    public TGridObject GetValue(Vector3 worldPosistion)
    {
        int x, y;
        GetXY(worldPosistion, out x, out y);
        return GetValue(x, y);
    }
    public void setDebug(bool b)
    {
        debugBool = b;
    }

}
