using UnityEngine;

public class Swap : MonoBehaviour
{
    [SerializeField] private GameObject belt;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite xBelt;
    [SerializeField] private Sprite pBelt;
    [SerializeField] public bool isCorrect;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new Vector2(mousePos.x, mousePos.y);

            var hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.transform.gameObject == belt)
            {
                isCorrect ^= true;
                ChangeSprite(spriteRenderer.sprite == xBelt ? pBelt : xBelt);
            }
        }
    }

    private void ChangeSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}