using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swap : MonoBehaviour
{
    public GameObject _gameObject;
    public SpriteRenderer _spriteRenderer;
    public Sprite _x_belt;
    public Sprite _p_belt;
    public bool _isCorrect;

    void Start()    
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    
    public void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.transform.gameObject == _gameObject)
            {
                _isCorrect ^= true;
                if (_spriteRenderer.sprite == _x_belt)
                    ChangeSprite(_p_belt);
                else
                    ChangeSprite(_x_belt);
            }
        }
    }
    
    void ChangeSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite; 
    }
}
