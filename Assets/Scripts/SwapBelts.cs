using UnityEngine;

public class SwapBelts : MonoBehaviour
{
    [SerializeField] private Sprite beltX;
    [SerializeField] private Sprite beltO;
    [SerializeField] public bool isCorrect;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new Vector2(mousePos.x, mousePos.y);

            var hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.transform.gameObject == gameObject)
            {
                isCorrect ^= true;
                _spriteRenderer.sprite = _spriteRenderer == beltX ? beltO : beltX;
                PlaySwitchSound();
            }
        }
    }

    private void PlaySwitchSound()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF")
            GetComponent<AudioSource>().Play();
    }
}