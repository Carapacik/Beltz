using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swap : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite x_belt;
    public Sprite p_belt;

    void ChangeSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite; 
    }

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (spriteRenderer.sprite == x_belt)
            {
                ChangeSprite(p_belt);
            }
            else
                ChangeSprite(x_belt);
        }
    }
}
