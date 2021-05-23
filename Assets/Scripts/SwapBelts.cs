using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class SwapBelts : MonoBehaviour
{
    [SerializeField] private Sprite beltX;
    [SerializeField] private Sprite beltO;
    [SerializeField] public bool isCorrect;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Assert(Camera.main != null, "Camera.main not null");
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new Vector2(mousePos.x, mousePos.y);

            var hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.transform.gameObject == gameObject)
            {
                isCorrect ^= true;
                PlaySwitchSound();
                ChangeSprite(_spriteRenderer.sprite == beltX ? beltO : beltX);
                _animator.SetTrigger("Swap");
            }
        }
    }

    private void ChangeSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    private void PlaySwitchSound()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF")
            GetComponent<AudioSource>().Play();
    }
}