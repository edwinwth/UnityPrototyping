using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrbTypes { Fire,Water,Wood,Light,Dark}

public class OrbBehavior : MonoBehaviour
{
    public OrbTypes OrbType { get; set; }
    public List<Sprite> orbs = new List<Sprite>();
    private bool clickedOn = false;
    private Vector3 mousePosition;
    private bool swapedPostion = false;
    private Vector3 originalPosition;
    public float snapSpeed;
    public float maxDragDistance;
    private float maxDragDistanceSqr;
    private EventManager script;
    private Vector3 originalScale;
    private bool matchFound = false;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        maxDragDistanceSqr = maxDragDistance * maxDragDistance;
        originalScale = transform.localScale;
        GameObject eventManager = GameObject.Find("EventManager");
        script = eventManager.GetComponent<EventManager>();
        setSprite();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        if (clickedOn)
        {
            dragging();
        }
        else
        {
            transform.localScale = originalScale;
            transform.position = Vector2.Lerp(transform.position, originalPosition, snapSpeed);
        }
        if (Input.GetMouseButtonUp(0))
        {
            ClearAllMatches();
        }
    }

    public List<Sprite> getOrbs()
    {
        return orbs;
    }

    public void setSwapBool(bool b)
    {
        swapedPostion = b;
    }

    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }
    public void SetOriginalPosition(Vector3 pos)
    {
        originalPosition = pos;
    }

    private void setSprite()
    {
        GetComponent<SpriteRenderer>().sprite = orbs[(int)OrbType];
    }
    private void OnMouseDown()
    {
        clickedOn = true;
    }

    private void OnMouseUp()
    {
        clickedOn = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (clickedOn)
        {
            GameObject hitOrb = collision.gameObject;
            if (hitOrb.tag == "Orb")
            {
                //Swap Grid Value (GameObject)
                int x, y;
                script.GetOrbGridXY(transform.position, out x, out y);

                int otherx, othery;
                script.GetOrbGridXY(hitOrb.transform.position, out otherx, out othery);

                GameObject tempValue = script.orbGrid[x, y].gameObject;
                script.setOrbGridValue(x, y, hitOrb);
                script.setOrbGridValue(otherx, othery, tempValue);
                //Swap Orb positions
                Vector3 tempPos = hitOrb.GetComponent<OrbBehavior>().GetOriginalPosition();
                OrbBehavior hitOrbScript = hitOrb.GetComponent<OrbBehavior>();
                hitOrbScript.SetOriginalPosition(originalPosition);
                originalPosition = tempPos;
                hitOrb.transform.position = Vector2.Lerp(hitOrb.transform.position, originalPosition, snapSpeed);


                swapedPostion = true;
                hitOrbScript.setSwapBool(true);
            }
        }
    }


    void dragging()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector3 orbPosition = transform.position;
        orbPosition.z = 0f;
        if (transform.localScale.sqrMagnitude < originalScale.sqrMagnitude * 1.1f)
            transform.localScale *= 1.1f;
        transform.position = mousePosition;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir, 2.5f);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == GetComponent<SpriteRenderer>().sprite)
        { 
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir, 2.5f);
        }
        return matchingTiles; 
    }


    private void ClearMatch(Vector2[] paths) 
    {
        List<GameObject> HorizontalMatchingTiles = new List<GameObject>();
        List<GameObject> VerticalMatchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++) 
        {
            if(i<2)
                HorizontalMatchingTiles.AddRange(FindMatch(paths[i]));
            else
                VerticalMatchingTiles.AddRange(FindMatch(paths[i]));
        }

        if (HorizontalMatchingTiles.Count >= 2) 
        {
            for (int i = 0; i < HorizontalMatchingTiles.Count; i++)
            {
                HorizontalMatchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true; 
        }

        if (VerticalMatchingTiles.Count >= 2)
        {
            for (int i = 0; i < VerticalMatchingTiles.Count; i++)
            {
                VerticalMatchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }
    }

    public void ClearAllMatches()
    {
        if (GetComponent<SpriteRenderer>().sprite == null)
            return;

        ClearMatch(new Vector2[4] { Vector2.left, Vector2.right, Vector2.up, Vector2.down });
        if (matchFound)
        {
            GetComponent<SpriteRenderer>().sprite = null;
            matchFound = false;
            
        }
        script.restartShifting();
    }

}
