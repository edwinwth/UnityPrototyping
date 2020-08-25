using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpType { attack,defend,heal};

public class PickUpBehavior : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    private PickUpType pickUpType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setSprite(int type)
    {
        GetComponent<SpriteRenderer>().sprite = sprites[type];
    }
    public PickUpType getPickUpType()
    {
        return pickUpType;
    }
    public void setPickUpType(PickUpType type)
    {
        pickUpType = type;
    }
}
